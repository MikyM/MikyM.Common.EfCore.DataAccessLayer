using System.Collections.Generic;
using MikyM.Common.DataAccessLayer.Repositories;
using MikyM.Common.Domain.Entities;
using MikyM.Common.EfCore.DataAccessLayer.Specifications;

namespace MikyM.Common.EfCore.DataAccessLayer.Repositories;

/// <summary>
/// Read-only repository
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="AggregateRootEntity"/></typeparam>
public interface IReadOnlyRepository<TEntity> : IRepositoryBase where TEntity : AggregateRootEntity
{
    /// <summary>
    /// Gets an entity based on given primary key values
    /// </summary>
    /// <param name="keyValues">Primary key values</param>
    /// <returns>Entity if found, null if not found</returns>
    ValueTask<TEntity?> GetAsync(params object[] keyValues);

    /// <summary>
    /// Gets a single (top 1) entity that satisfies given <see cref="ISpecification{T}"/>
    /// </summary>
    /// <param name="specification">Specification for the query</param>
    /// <returns>Entity if found, null if not found</returns>
    Task<TEntity?> GetSingleBySpecAsync(ISpecification<TEntity> specification);

    /// <summary>
    /// Gets a single (top 1) entity that satisfies <see cref="ISpecification{T,TProjectTo}"/> and projects it to another entity
    /// </summary>
    /// <param name="specification">Specification for the query</param>
    /// <returns>Entity if found, null if not found</returns>
    Task<TProjectTo?> GetSingleBySpecAsync<TProjectTo>(ISpecification<TEntity, TProjectTo> specification)
        where TProjectTo : class;

    /// <summary>
    /// Gets all entities that satisfy given <see cref="ISpecification{T}"/>
    /// </summary>
    /// <param name="specification">Specification for the query</param>
    /// <returns><see cref="IReadOnlyList{T}"/> with found entities</returns>
    Task<IReadOnlyList<TEntity>> GetBySpecAsync(ISpecification<TEntity> specification);

    /// <summary>
    /// Gets all entities that satisfy <see cref="ISpecification{T,TProjectTo}"/> and projects them to another entities 
    /// </summary>
    /// <param name="specification">Specification for the query</param>
    /// <returns><see cref="IReadOnlyList{T}"/> with found entities</returns>
    Task<IReadOnlyList<TProjectTo>> GetBySpecAsync<TProjectTo>(ISpecification<TEntity, TProjectTo> specification)
        where TProjectTo : class;

    /// <summary>
    /// Gets all entities
    /// </summary>
    /// <returns><see cref="IReadOnlyList{T}"/> with all entities</returns>
    Task<IReadOnlyList<TEntity>> GetAllAsync();

    /// <summary>
    /// Gets all entities and projects them to another entity
    /// </summary>
    /// <returns><see cref="IReadOnlyList{T}"/> with all entities</returns>
    Task<IReadOnlyList<TProjectTo>> GetAllAsync<TProjectTo>() where TProjectTo : class;

    /// <summary>
    /// Counts the entities, optionally using a provided <see cref="ISpecification{T}"/>
    /// </summary>
    /// <param name="specification">Specification for the query</param>
    /// <returns>Number of entities that satisfy given <see cref="ISpecification{T}"/></returns>
    Task<long> LongCountAsync(ISpecification<TEntity>? specification = null);
}