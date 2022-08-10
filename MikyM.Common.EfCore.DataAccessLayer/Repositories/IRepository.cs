using System.Collections.Generic;
using System.Threading;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MikyM.Common.EfCore.DataAccessLayer.Repositories;

/// <summary>
/// Repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entity{TId}"/>.</typeparam>
/// <typeparam name="TId">Type of the Id in <typeparamref name="TEntity"/>.</typeparam>
[PublicAPI]
public interface IRepository<TEntity,TId> : IReadOnlyRepository<TEntity,TId> where TEntity : class, IEntity<TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <inheritdoc cref="DbSet{TEntity}.Add"/>
    void Add(TEntity entity);

    /// <inheritdoc cref="DbSet{TEntity}.AddRange(IEnumerable{TEntity})"/>
    void AddRange(IEnumerable<TEntity> entities);
    
    /// <inheritdoc cref="DbSet{TEntity}.AddRange(TEntity[])"/>
    void AddRange(params TEntity[] entities);
    
    /// <inheritdoc cref="DbSet{TEntity}.AddAsync"/>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="DbSet{TEntity}.AddRangeAsync(IEnumerable{TEntity}, CancellationToken)"/>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    /// <inheritdoc cref="DbSet{TEntity}.AddRangeAsync(TEntity[])"/>
    Task AddRangeAsync(params TEntity[] entities);

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

    /// <inheritdoc cref="DbSet{TEntity}.Remove"/>
    void Delete(TEntity entity);

    /// <summary>
    ///     Tries to find a tracked entity by given Id or attempts to create a shell instance of it.
    ///     Begins tracking the given entity in the <see cref="EntityState.Deleted" /> state such that it will
    ///     be removed from the database when <see cref="DbContext.SaveChanges()" /> is called.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If the entity is already tracked in the <see cref="EntityState.Added" /> state then the context will
    ///         stop tracking the entity (rather than marking it as <see cref="EntityState.Deleted" />) since the
    ///         entity was previously added to the context and does not exist in the database.
    ///     </para>
    ///     <para>
    ///         Any other reachable entities that are not already being tracked will be tracked in the same way that
    ///         they would be if <see cref="DbSet.Attach(TEntity)" /> was called before calling this method.
    ///         This allows any cascading actions to be applied when <see cref="DbContext.SaveChanges()" /> is called.
    ///     </para>
    ///     <para>
    ///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information.
    ///     </para>
    /// </remarks>
    /// <param name="id">The Id of the entity to remove.</param>
    /// <returns>
    ///     The <see cref="EntityEntry{TEntity}" /> for the entity. The entry provides
    ///     access to change tracking information and operations for the entity.
    /// </returns>
    void Delete(TId id);

    /// <inheritdoc cref="DbSet{TEntity}.RemoveRange(IEnumerable{TEntity})"/>
    void DeleteRange(IEnumerable<TEntity> entities);

    /// <summary>
    ///     Tries to find tracked entities by given Ids or attempts to create shell instances of them.
    ///     Begins tracking the given entities in the <see cref="EntityState.Deleted" /> state such that they will
    ///     be removed from the database when <see cref="DbContext.SaveChanges()" /> is called.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If any of the entities are already tracked in the <see cref="EntityState.Added" /> state then the context will
    ///         stop tracking those entities (rather than marking them as <see cref="EntityState.Deleted" />) since those
    ///         entities were previously added to the context and do not exist in the database.
    ///     </para>
    ///     <para>
    ///         Any other reachable entities that are not already being tracked will be tracked in the same way that
    ///         they would be if <see cref="AttachRange(IEnumerable{TEntity})" /> was called before calling this method.
    ///         This allows any cascading actions to be applied when <see cref="DbContext.SaveChanges()" /> is called.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see>
    ///         and <see href="https://aka.ms/efcore-docs-attach-range">Using AddRange, UpdateRange, AttachRange, and RemoveRange</see>
    ///         for more information.
    ///     </para>
    /// </remarks>
    /// <param name="ids">The Ids of the entities to remove.</param>
    void DeleteRange(IEnumerable<TId> ids);

    /// <summary>
    /// Disables an entity.
    /// </summary>
    /// <param name="entity">Entity to disable.</param>
    void Disable(TEntity entity);

    /// <summary>
    /// Disables an entity.
    /// </summary>
    /// <param name="id">Id of the entity to disable.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DisableAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables a range of entities.
    /// </summary>
    /// <param name="entities">Entities to disable.</param>
    void DisableRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Disables a range of entities.
    /// </summary>
    /// <param name="ids">Ids of the entities to disable.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DisableRangeAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Detaches an entity and all of it's reachable navigation properties.
    /// </summary>
    /// <param name="entity">Entity to detach.</param>
    void Detach(TEntity entity);
}

/// <summary>
/// Repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entity"/>.</typeparam>
[PublicAPI]
public interface IRepository<TEntity> : IRepository<TEntity,long> where TEntity : class, IEntity<long>
{
}
