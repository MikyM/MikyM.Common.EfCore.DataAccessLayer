using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using MikyM.Common.EfCore.DataAccessLayer.Helpers;
using MikyM.Common.EfCore.DataAccessLayer.Repositories;
using MikyM.Common.EfCore.DataAccessLayer.Specifications.Evaluators;

namespace MikyM.Common.EfCore.DataAccessLayer.UnitOfWork;

/// <inheritdoc cref="IUnitOfWork{TContext}"/>
[PublicAPI]
public sealed class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : IEfDbContext
{
    /// <summary>
    /// Inner <see cref="ISpecificationEvaluator"/>.
    /// </summary>
    private readonly ISpecificationEvaluator _specificationEvaluator;
    /// <summary>
    /// Configuration.
    /// </summary>
    private readonly IOptions<EfCoreDataAccessConfiguration> _options;

    // To detect redundant calls
    private bool _disposed;
    // ReSharper disable once InconsistentNaming

    /// <summary>
    /// Repository cache.
    /// </summary>
    private ConcurrentDictionary<string, Lazy<IRepositoryBase>>? _repositories;
    /// <summary>
    /// Repository entity type cache.
    /// </summary>
    private ConcurrentDictionary<string, string>? _entityTypesOfRepositories;

    /// <summary>
    /// Inner <see cref="IDbContextTransaction"/>.
    /// </summary>
    private IDbContextTransaction? _transaction;

    /// <summary>
    /// Creates a new instance of <see cref="UnitOfWork{TContext}"/>.
    /// </summary>
    /// <param name="context"><see cref="DbContext"/> to be used.</param>
    /// <param name="specificationEvaluator">Specification evaluator to be used.</param>
    /// <param name="options">Options.</param>
    public UnitOfWork(TContext context, ISpecificationEvaluator specificationEvaluator, IOptions<EfCoreDataAccessConfiguration> options)
    {
        Context = context;
        _specificationEvaluator = specificationEvaluator;
        _options = options;
    }

    /// <inheritdoc />
    public TContext Context { get; }

    /// <inheritdoc />
    public async Task UseTransactionAsync(CancellationToken cancellationToken = default)
        => _transaction ??= await Context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

    /// <inheritdoc cref="IUnitOfWork.GetRepository{TRepository}" />
    public TRepository GetRepository<TRepository>() where TRepository : class, IRepositoryBase
    {
        var givenType = typeof(TRepository);
        if (!givenType.IsInterface || !givenType.IsGenericType)
            throw new NotSupportedException("You can only retrieve types: IRepository<TEntity>, IRepository<TEntity,TId>, IReadOnlyRepository<TEntity> and IReadOnlyRepository<TEntity,TId>.");

        _repositories ??= new ConcurrentDictionary<string, Lazy<IRepositoryBase>>();
        _entityTypesOfRepositories ??= new ConcurrentDictionary<string, string>();

        Type? repositoryType;
        string repositoryTypeFullName;
        
        var entityType = givenType.GetGenericArguments().FirstOrDefault();
        if (entityType is null)
            throw new ArgumentException("Couldn't retrieve entity type from generic arguments on given repository type.");
        var entityTypeName = entityType.FullName ?? entityType.Name;
        
        switch (givenType.IsGenericType)
        {
            case true when givenType.GetGenericTypeDefinition() == typeof(IRepository<,>):
                repositoryType = UoFCache.CachedCrudGenericIdRepos.GetValueOrDefault(entityType);
                repositoryTypeFullName = repositoryType?.FullName ?? throw new InvalidOperationException("Couldn't find proper type in cache.");
                break;
            case true when givenType.GetGenericTypeDefinition() == typeof(IReadOnlyRepository<,>):
                repositoryType = UoFCache.CachedReadOnlyGenericIdRepos.GetValueOrDefault(entityType);
                repositoryTypeFullName = repositoryType?.FullName ?? throw new InvalidOperationException("Couldn't find proper type in cache.");
                break;
            case true when givenType.GetGenericTypeDefinition() == typeof(IRepository<>):
                repositoryType = UoFCache.CachedCrudRepos.GetValueOrDefault(entityType);
                repositoryTypeFullName = repositoryType?.FullName ?? throw new InvalidOperationException("Couldn't find proper type in cache.");
                break;
            case true when givenType.GetGenericTypeDefinition() == typeof(IReadOnlyRepository<>):
                repositoryType = UoFCache.CachedReadOnlyRepos.GetValueOrDefault(entityType);
                repositoryTypeFullName = repositoryType?.FullName ?? throw new InvalidOperationException("Couldn't find proper type in cache.");
                break;
            default:
                throw new NotSupportedException("You can only retrieve types: IRepository<TEntity>, IRepository<TEntity,TId>, IReadOnlyRepository<TEntity> and IReadOnlyRepository<TEntity,TId>.");
        }
        
        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");

        // lazy to avoid creating whole repository and then discarding it due to the fact that GetOrAdd isn't atomic, creation of Lazy<T> is very cheap
        var lazyRepository = _repositories.GetOrAdd(repositoryTypeFullName, _ =>
        {
            if (!_entityTypesOfRepositories.TryAdd(entityTypeName, entityTypeName))
                throw new InvalidOperationException(
                    "Seems like you tried to create a different type of repository (ie. both read-only and crud) for same entity type within same unit of work instance - it is not supported as it may lead to unexpected results and it is not advised as it makes code less readable.");

            return new Lazy<IRepositoryBase>(() =>
            {
                var instance =
                    InstanceFactory.CreateInstance(repositoryType, Context,
                        _specificationEvaluator);

                if (instance is null) 
                    throw new InvalidOperationException($"Couldn't create an instance of {repositoryTypeFullName}");

                return (TRepository)instance;
            });
        });

        return (TRepository)lazyRepository.Value;
    }

    /// <inheritdoc />
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null) 
            await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_options.Value.OnBeforeSaveChangesActions is not null &&
            _options.Value.OnBeforeSaveChangesActions.TryGetValue(typeof(TContext).Name, out var action))
            await action.Invoke(this);
        
        _ = await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        
        if (_transaction is not null) 
            await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task CommitAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (_options.Value.OnBeforeSaveChangesActions is not null &&
            _options.Value.OnBeforeSaveChangesActions.TryGetValue(typeof(TContext).Name, out var action))
            await action.Invoke(this);

        if (Context is AuditableDbContext auditableDbContext)
            _ = await auditableDbContext.SaveChangesAsync(userId, cancellationToken).ConfigureAwait(false);
        else
            _ = await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        
        if (_transaction is not null) 
            await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
    
    /// <inheritdoc />
    public async Task<int> CommitWithCountAsync(CancellationToken cancellationToken = default)
    {
        if (_options.Value.OnBeforeSaveChangesActions is not null &&
            _options.Value.OnBeforeSaveChangesActions.TryGetValue(typeof(TContext).Name, out var action))
            await action.Invoke(this);

        int result = await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        if (_transaction is not null) 
            await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        return result;
    }

    /// <inheritdoc />
    public async Task<int> CommitWithCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (_options.Value.OnBeforeSaveChangesActions is not null &&
            _options.Value.OnBeforeSaveChangesActions.TryGetValue(typeof(TContext).Name, out var action))
            await action.Invoke(this);

        int result;
        if (Context is AuditableDbContext auditableDbContext)
            result = await auditableDbContext.SaveChangesAsync(userId, cancellationToken).ConfigureAwait(false);
        else
            result = await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (_transaction is not null) 
            await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        
        return result;
    }

    // Public implementation of Dispose pattern callable by consumers.
    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            Context.Dispose();
            _transaction?.Dispose();
        }

        _repositories = null;
        _entityTypesOfRepositories = null;

        _disposed = true;
    }
}
