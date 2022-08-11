using System.Threading;
using MikyM.Common.DataAccessLayer;
using MikyM.Common.EfCore.DataAccessLayer.Repositories;

namespace MikyM.Common.EfCore.DataAccessLayer.UnitOfWork;

/// <summary>
/// Defines an EF Unit of Work.
/// </summary>
[PublicAPI]
public interface IUnitOfWork : IUnitOfWorkBase
{
    /// <summary>
    /// Begins a transaction.
    /// </summary>
    /// <returns>Task representing the asynchronous operation.</returns>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UseTransactionAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc cref="IUnitOfWorkBase.CommitAsync(string, CancellationToken)"/>
    /// <returns>Number of affected rows.</returns>
    /// <param name="auditUserId">The ID of the user responsible for the changes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<int> CommitWithCountAsync(string auditUserId, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="IUnitOfWorkBase.CommitAsync(CancellationToken)"/>
    /// <returns>Number of affected rows.</returns>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<int> CommitWithCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a repository of a given type.
    /// </summary>
    /// <remarks>You can <b>only</b> retrieve types: <see cref="IRepository{TEntity}"/>, <see cref="IRepository{TEntity,TId}"/>, <see cref="IReadOnlyRepository{TEntity}"/> and <see cref="IReadOnlyRepository{TEntity,TId}"/>.</remarks>
    /// <typeparam name="TRepository">Type of the repository to get.</typeparam>
    /// <returns>The searched for repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when couldn't find proper type or name in cache.</exception>
    /// <exception cref="NotSupportedException">Thrown when passed type isn't equal to any of the types listed in remarks, isn't a generic type or isn't an interface.</exception>
    new TRepository GetRepository<TRepository>() where TRepository : class, IRepositoryBase;

    /// <summary>
    /// Gets an <see cref="IRepository{TEntity}"/> for an entity of a given type.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity for which the repository should be retrieved.</typeparam>
    /// <returns>The searched for repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when couldn't find proper type or name in cache.</exception>
    IRepository<TEntity> GetRepositoryFor<TEntity>() where TEntity : Entity<long>;
    
    /// <summary>
    /// Gets an <see cref="IReadOnlyRepository{TEntity}"/> for an entity of a given type.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity for which the repository should be retrieved.</typeparam>
    /// <returns>The searched for repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when couldn't find proper type or name in cache.</exception>
    IReadOnlyRepository<TEntity> GetReadOnlyRepositoryFor<TEntity>() where TEntity : Entity<long>;

    /// <summary>
    /// Gets an <see cref="IRepository{TEntity,TId}"/> for an entity of a given type and Id type.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity for which the repository should be retrieved.</typeparam>
    /// <typeparam name="TId">Type of the Id of the entity.</typeparam>
    /// <returns>The searched for repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when couldn't find proper type or name in cache.</exception>
    IRepository<TEntity, TId> GetRepositoryFor<TEntity, TId>() where TEntity : Entity<TId>
        where TId : IComparable, IEquatable<TId>, IComparable<TId>;
    
    /// <summary>
    /// Gets an <see cref="IReadOnlyRepository{TEntity,TId}"/> for an entity of a given type and Id type.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity for which the repository should be retrieved.</typeparam>
    /// <typeparam name="TId">Type of the Id of the entity.</typeparam>
    /// <returns>The searched for repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when couldn't find proper type or name in cache.</exception>
    IReadOnlyRepository<TEntity, TId> GetReadOnlyRepositoryFor<TEntity, TId>() where TEntity : Entity<TId>
        where TId : IComparable, IEquatable<TId>, IComparable<TId>;
}

/// <inheritdoc cref="IUnitOfWork"/>
/// <summary>
/// Defines an EF Unit of Work.
/// </summary>
/// <typeparam name="TContext">Type of context to be used.</typeparam>
[PublicAPI]
public interface IUnitOfWork<out TContext> : IUnitOfWork where TContext : IEfDbContext
{
    /// <summary>
    /// Current <see cref="DbContext"/>.
    /// </summary>
    TContext Context { get; }
}
