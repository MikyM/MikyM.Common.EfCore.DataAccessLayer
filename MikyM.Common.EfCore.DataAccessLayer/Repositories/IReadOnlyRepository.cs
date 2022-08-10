using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;

namespace MikyM.Common.EfCore.DataAccessLayer.Repositories;

/// <summary>
/// Read-only repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entity{TId}"/>.</typeparam>
/// <typeparam name="TId">Type of the Id in <typeparamref name="TEntity"/>.</typeparam>
[PublicAPI]
public interface IReadOnlyRepository<TEntity,TId> : IRepositoryBase where TEntity : class, IEntity<TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    /// Current <see cref="IEfDbContext"/>.
    /// </summary>
    IEfDbContext Context { get; }
    
    /// <summary>
    /// Current <see cref="DbSet{TEntity}"/>.
    /// </summary>
    DbSet<TEntity> Set { get; }
    
    /// <summary>
    /// Gets an entity based on given primary key values.
    /// </summary>
    /// <param name="keyValues">Primary key values.</param>
    /// <returns>Entity if found, null if not found.</returns>
    ValueTask<TEntity?> GetAsync(params object[] keyValues);
    
    /// <summary>
    /// Gets an entity based on given primary key values.
    /// </summary>
    /// <param name="keyValues">Primary key values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Entity if found, null if not found.</returns>
    ValueTask<TEntity?> GetAsync(object?[]? keyValues, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a single (top 1) entity that satisfies given <see cref="ISpecification{T}"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns>Entity if found, null if not found.</returns>
    Task<TEntity?> GetSingleBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single (top 1) entity that satisfies <see cref="ISpecification{T,TProjectTo}"/> and projects it to another entity.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns>Entity if found, null if not found.</returns>
    Task<TProjectTo?> GetSingleBySpecAsync<TProjectTo>(ISpecification<TEntity, TProjectTo> specification, CancellationToken cancellationToken = default)
        where TProjectTo : class;

    /// <summary>
    /// Gets all entities that satisfy given <see cref="ISpecification{T}"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns><see cref="IReadOnlyList{T}"/> with found entities.</returns>
    Task<IReadOnlyList<TEntity>> GetBySpecAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities that satisfy <see cref="ISpecification{T,TProjectTo}"/> and projects them to another entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="specification">Specification for the query.</param>
    /// <returns><see cref="IReadOnlyList{T}"/> with found entities.</returns>
    Task<IReadOnlyList<TProjectTo>> GetBySpecAsync<TProjectTo>(ISpecification<TEntity, TProjectTo> specification, CancellationToken cancellationToken = default)
        where TProjectTo : class;

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="IReadOnlyList{T}"/> with all entities.</returns>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities and projects them to another entity.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="IReadOnlyList{T}"/> with all entities.</returns>
    Task<IReadOnlyList<TProjectTo>> GetAllAsync<TProjectTo>(CancellationToken cancellationToken = default) where TProjectTo : class;

    /// <summary>
    /// Counts the entities that satisfy the given <see cref="ISpecification{T}"/>.
    /// </summary>
    /// <param name="specification">Specification for the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of entities that satisfy given <see cref="ISpecification{T}"/>.</returns>
    Task<long> LongCountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts the entities, optionally using a provided <see cref="ISpecification{T}"/>.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of entities that satisfy given <see cref="ISpecification{T}"/>.</returns>
    Task<long> LongCountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Asynchronously determines whether any elements satisfy the given condition.
    /// </summary>
    /// <param name="predicate">Predicate for the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if any elements in the source sequence satisfy the condition, otherwise false.</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity,bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously determines whether any elements satisfy the given <see cref="ISpecification{TEntity}"/>.
    /// </summary>
    /// <param name="specification">Specification for the query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if any elements in the source sequence satisfy the condition, otherwise false.</returns>
    Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entity"/>.</typeparam>
[PublicAPI]
public interface IReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity,long> where TEntity : class, IEntity<long>
{
}
