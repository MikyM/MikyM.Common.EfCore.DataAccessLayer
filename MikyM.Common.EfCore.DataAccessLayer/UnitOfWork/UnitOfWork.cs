using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using MikyM.Common.EfCore.DataAccessLayer.Helpers;
using MikyM.Common.EfCore.DataAccessLayer.Repositories;
using MikyM.Common.EfCore.DataAccessLayer.Specifications.Evaluators;
using MikyM.Common.Utilities.Extensions;

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
    private ConcurrentDictionary<RepositoryEntryKey, Lazy<RepositoryEntry>>? _repositories;

    /// <summary>
    /// Allowed repo types.
    /// </summary>
    private Type[] _allowedRepoTypes =
    {
        typeof(IRepository<>), typeof(IRepository<,>), typeof(IReadOnlyRepository<>), typeof(IReadOnlyRepository<,>)
    };

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

    /// <inheritdoc cref="IUnitOfWork.GetRepositoryFor{TRepository}" />
    public IRepository<TEntity> GetRepositoryFor<TEntity>() where TEntity : Entity<long>
    {
        var entityType = typeof(TEntity);
        var repositoryType = UoFCache.CachedCrudRepos.GetValueOrDefault(entityType);

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");

        return LazilyGetOrCreateRepository<IRepository<TEntity>>(repositoryType, entityType, true);
    }

    /// <inheritdoc cref="IUnitOfWork.GetReadOnlyRepositoryFor{TRepository}" />
    public IReadOnlyRepository<TEntity> GetReadOnlyRepositoryFor<TEntity>() where TEntity : Entity<long>
    {
        var entityType = typeof(TEntity);
        var repositoryType = UoFCache.CachedReadOnlyRepos.GetValueOrDefault(entityType);

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");

        return LazilyGetOrCreateRepository<IReadOnlyRepository<TEntity>>(repositoryType, entityType, false);
    }

    /// <inheritdoc cref="IUnitOfWork.GetRepositoryFor{TRepository,TId}" />
    public IRepository<TEntity, TId> GetRepositoryFor<TEntity, TId>() where TEntity : Entity<TId>
        where TId : IComparable, IEquatable<TId>, IComparable<TId>
    {
        var entityType = typeof(TEntity);
        var repositoryType = UoFCache.CachedCrudGenericIdRepos.GetValueOrDefault(entityType);

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");

        return LazilyGetOrCreateRepository<IRepository<TEntity, TId>>(repositoryType, entityType, true);
    }

    /// <inheritdoc cref="IUnitOfWork.GetReadOnlyRepositoryFor{TRepository,TId}" />
    public IReadOnlyRepository<TEntity, TId> GetReadOnlyRepositoryFor<TEntity, TId>() where TEntity : Entity<TId>
        where TId : IComparable, IEquatable<TId>, IComparable<TId>
    {
        var entityType = typeof(TEntity);
        var repositoryType = UoFCache.CachedReadOnlyGenericIdRepos.GetValueOrDefault(entityType);

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");

        return LazilyGetOrCreateRepository<IReadOnlyRepository<TEntity, TId>>(repositoryType, entityType, false);
    }

    /// <inheritdoc cref="IUnitOfWork.GetRepository{TRepository}" />
    public TRepository GetRepository<TRepository>() where TRepository : class, IRepositoryBase
    {
        var givenType = typeof(TRepository);
        if (!givenType.IsInterface || !givenType.IsGenericType ||
            !_allowedRepoTypes.Contains(givenType.GetGenericTypeDefinition()))
            throw new NotSupportedException(
                "You can only retrieve types: IRepository<TEntity>, IRepository<TEntity,TId>, IReadOnlyRepository<TEntity> and IReadOnlyRepository<TEntity,TId>.");

        var entityType = givenType.GetGenericArguments().FirstOrDefault();
        if (entityType is null)
            throw new ArgumentException(
                "Couldn't retrieve entity type from generic arguments on given repository type.");

        Type? repositoryType;
        bool isCrud;
        switch (givenType.IsGenericType)
        {
            case true when givenType.GetGenericTypeDefinition() == typeof(IRepository<,>):
                repositoryType = UoFCache.CachedCrudGenericIdRepos.GetValueOrDefault(entityType);
                isCrud = true;
                break;
            case true when givenType.GetGenericTypeDefinition() == typeof(IReadOnlyRepository<,>):
                repositoryType = UoFCache.CachedReadOnlyGenericIdRepos.GetValueOrDefault(entityType);
                isCrud = false;
                break;
            case true when givenType.GetGenericTypeDefinition() == typeof(IRepository<>):
                repositoryType = UoFCache.CachedCrudRepos.GetValueOrDefault(entityType);
                isCrud = true;
                break;
            case true when givenType.GetGenericTypeDefinition() == typeof(IReadOnlyRepository<>):
                repositoryType = UoFCache.CachedReadOnlyRepos.GetValueOrDefault(entityType);
                isCrud = false;
                break;
            default:
                throw new NotSupportedException(
                    "You can only retrieve types: IRepository<TEntity>, IRepository<TEntity,TId>, IReadOnlyRepository<TEntity> and IReadOnlyRepository<TEntity,TId>.");
        };

        if (repositoryType is null)
            throw new InvalidOperationException("Couldn't find proper type in cache.");

        return LazilyGetOrCreateRepository<TRepository>(repositoryType, entityType, isCrud);
    }

    /// <summary>
    /// Lazily creates a new repository instance of a given type.
    /// </summary>
    /// <param name="repositoryType">Repository closed generic type.</param>
    /// <param name="entityType">Entity type.</param>
    /// <param name="isCrud">Whether the repository is a crud repository.</param>
    /// <typeparam name="TRepository">Type of the wanted repository</typeparam>
    /// <returns>Created repo instance.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private TRepository LazilyGetOrCreateRepository<TRepository>(Type repositoryType, Type entityType, bool isCrud) where TRepository : IRepositoryBase
    {
        _repositories ??= new ConcurrentDictionary<RepositoryEntryKey, Lazy<RepositoryEntry>>();

        var repositoryTypeName = repositoryType.FullName ?? repositoryType.Name;
        var entityTypeName = entityType.FullName ?? entityType.Name;

        var key = new RepositoryEntryKey(entityTypeName, isCrud);

        var repositoryEntry = _repositories.GetOrAdd(key, new Lazy<RepositoryEntry>(() =>
        {
            var lazyRepo = new Lazy<IRepositoryBase>(() =>
            {
                var instance =
                    InstanceFactory.CreateInstance(repositoryType, Context,
                        _specificationEvaluator);

                if (instance is null)
                    throw new InvalidOperationException($"Couldn't create an instance of {repositoryTypeName}");

                return (TRepository)instance;
            }, LazyThreadSafetyMode.ExecutionAndPublication);

            return new RepositoryEntry(entityType, repositoryType, lazyRepo, isCrud);
        }, LazyThreadSafetyMode.ExecutionAndPublication));

        return (TRepository)repositoryEntry.Value.LazyRepo.Value;
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

        _disposed = true;
    }
}

/// <summary>
/// Repository entry.
/// </summary>
internal class RepositoryEntry
{
    internal RepositoryEntry(Type entityType, Type repoClosedGenericType, Lazy<IRepositoryBase> lazyRepo, bool isCrud)
    {
        EntityType = entityType;
        RepoClosedGenericType = repoClosedGenericType;
        IsCrud = isCrud;
        LazyRepo = lazyRepo;
    }
    internal Type EntityType { get; }
    internal Type RepoClosedGenericType { get; }
    internal bool IsCrud { get; }
    internal Lazy<IRepositoryBase> LazyRepo { get; }
}

/// <summary>
/// Repository entry key.
/// </summary>
internal readonly struct RepositoryEntryKey : IEquatable<RepositoryEntryKey>
{
    internal RepositoryEntryKey(string entityTypeName, bool isCrud)
    {
        EntityTypeName = entityTypeName;
        IsCrud = isCrud;
    }

    private string EntityTypeName { get; }
    private bool IsCrud { get; }

    public bool Equals(RepositoryEntryKey other)
        => EntityTypeName == other.EntityTypeName && IsCrud == other.IsCrud;

    public override bool Equals(object? obj)
        => obj is RepositoryEntryKey other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(EntityTypeName, IsCrud);
}
