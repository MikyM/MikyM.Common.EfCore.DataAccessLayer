using System.Collections.Generic;
using MikyM.Common.Domain.Entities;
using MikyM.Common.Domain.Entities.Base;

namespace MikyM.Common.EfCore.DataAccessLayer.Repositories;

/// <summary>
/// Repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entity"/>.</typeparam>
[PublicAPI]
public interface IRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class, IEntityBase
{
    /// <summary>
    /// Adds an entity.
    /// </summary>
    /// <param name="entity">Entity to add.</param>
    void Add(TEntity entity);

    /// <summary>
    /// Adds a range of entities.
    /// </summary>
    /// <param name="entities">Entities to add.</param>
    void AddRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Begins updating an entity.
    /// </summary>
    /// <param name="entity">Entity to track.</param>
    /// <param name="shouldSwapAttached">Whether to swap attached entity if one with same primary key is already attached to <see cref="DbContext"/>.</param>
    void BeginUpdate(TEntity entity, bool shouldSwapAttached = false);

    /// <summary>
    /// Begins updating a range of entities.
    /// </summary>
    /// <param name="entities">Entities to track.</param>
    /// <param name="shouldSwapAttached">Whether to swap attached entities if entities with same primary keys are already attached to <see cref="DbContext"/>.</param>
    void BeginUpdateRange(IEnumerable<TEntity> entities, bool shouldSwapAttached = false);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">Entity to delete.</param>
    void Delete(TEntity entity);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="id">Id of the entity to delete.</param>
    void Delete(long id);

    /// <summary>
    /// Deletes a range of entities.
    /// </summary>
    /// <param name="entities">Entities to delete.</param>
    void DeleteRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Deletes a range of entities.
    /// </summary>
    /// <param name="ids">Ids of the entities to delete.</param>
    void DeleteRange(IEnumerable<long> ids);

    /// <summary>
    /// Disables an entity.
    /// </summary>
    /// <param name="entity">Entity to disable.</param>
    void Disable(TEntity entity);

    /// <summary>
    /// Disables an entity.
    /// </summary>
    /// <param name="id">Id of the entity to disable.</param>
    Task DisableAsync(long id);

    /// <summary>
    /// Disables a range of entities.
    /// </summary>
    /// <param name="entities">Entities to disable.</param>
    void DisableRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Disables a range of entities.
    /// </summary>
    /// <param name="ids">Ids of the entities to disable.</param>
    Task DisableRangeAsync(IEnumerable<long> ids);

    /// <summary>
    /// Detaches an entity and all of it's reachable navigation properties.
    /// </summary>
    /// <param name="entity">Entity to detach.</param>
    void Detach(TEntity entity);
}
