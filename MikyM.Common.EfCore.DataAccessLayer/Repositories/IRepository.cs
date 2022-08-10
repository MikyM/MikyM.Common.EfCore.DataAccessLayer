using System.Collections.Generic;
using System.Threading;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MikyM.Common.DataAccessLayer.Exceptions;

namespace MikyM.Common.EfCore.DataAccessLayer.Repositories;

/// <summary>
/// Repository.
/// </summary>
/// <typeparam name="TEntity">Entity that derives from <see cref="Entity{TId}"/>.</typeparam>
/// <typeparam name="TId">Type of the Id in <typeparamref name="TEntity"/>.</typeparam>
[PublicAPI]
public interface IRepository<TEntity,TId> : IReadOnlyRepository<TEntity,TId> where TEntity : class, IEntity<TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
{
    /// <summary>
    ///     <para>
    ///         Begins tracking the given entity, and any other reachable entities that are
    ///         not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
    ///         be inserted into the database when <see cref="DbContext.SaveChanges()" /> is called.
    ///     </para>
    ///     <para>
    ///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information.
    /// </remarks>
    /// <param name="entity">The entity to add.</param>
    /// <returns>
    ///     The <see cref="EntityEntry{TEntity}" /> for the entity. The entry provides
    ///     access to change tracking information and operations for the entity.
    /// </returns>
    void Add(TEntity entity);

    /// <summary>
    ///     Begins tracking the given entities, and any other reachable entities that are
    ///     not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
    ///     be inserted into the database when <see cref="DbContext.SaveChanges()" /> is called.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see>
    ///     and <see href="https://aka.ms/efcore-docs-attach-range">Using AddRange, UpdateRange, AttachRange, and RemoveRange</see>
    ///     for more information.
    /// </remarks>
    /// <param name="entities">The entities to add.</param>
    void AddRange(IEnumerable<TEntity> entities);
    
    /// <summary>
    ///     Begins tracking the given entities, and any other reachable entities that are
    ///     not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
    ///     be inserted into the database when <see cref="DbContext.SaveChanges()" /> is called.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see>
    ///     and <see href="https://aka.ms/efcore-docs-attach-range">Using AddRange, UpdateRange, AttachRange, and RemoveRange</see>
    ///     for more information.
    /// </remarks>
    /// <param name="entities">The entities to add.</param>
    void AddRange(params TEntity[] entities);
    
    /// <summary>
    ///     <para>
    ///         Begins tracking the given entity, and any other reachable entities that are
    ///         not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
    ///         be inserted into the database when <see cref="DbContext.SaveChanges()" /> is called.
    ///     </para>
    ///     <para>
    ///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information.
    /// </remarks>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous Add operation. The task result contains the
    ///     <see cref="EntityEntry{TEntity}" /> for the entity. The entry provides access to change tracking
    ///     information and operations for the entity.
    /// </returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     <para>
    ///         Begins tracking the given entities, and any other reachable entities that are
    ///         not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
    ///         be inserted into the database when <see cref="DbContext.SaveChanges()" /> is called.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see>
    ///     and <see href="https://aka.ms/efcore-docs-attach-range">Using AddRange, UpdateRange, AttachRange, and RemoveRange</see>
    ///     for more information.
    /// </remarks>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    /// <summary>
    ///     <para>
    ///         Begins tracking the given entities, and any other reachable entities that are
    ///         not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
    ///         be inserted into the database when <see cref="DbContext.SaveChanges()" /> is called.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see>
    ///     and <see href="https://aka.ms/efcore-docs-attach-range">Using AddRange, UpdateRange, AttachRange, and RemoveRange</see>
    ///     for more information.
    /// </remarks>
    /// <param name="entities">The entities to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddRangeAsync(params TEntity[] entities);

    /// <summary>
    ///     <para>
    ///         Begins tracking the given entity and entries reachable from the given entity using
    ///         the <see cref="EntityState.Unchanged" /> state by default, but see below for cases
    ///         when a different state will be used.
    ///     </para>
    ///     <para>
    ///         Generally, no database interaction will be performed until <see cref="SaveChangesAsync()" /> or <see cref="SaveChanges()"/> is called.
    ///     </para>
    ///     <para>
    ///         A recursive search of the navigation properties will be performed to find reachable entities
    ///         that are not already being tracked by the context. All entities found will be tracked
    ///         by the context.
    ///     </para>
    ///     <para>
    ///         For entity types with generated keys if an entity has its primary key value set
    ///         then it will be tracked in the <see cref="EntityState.Unchanged" /> state. If the primary key
    ///         value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
    ///         This helps ensure only new entities will be inserted.
    ///         An entity is considered to have its primary key value set if the primary key property is set
    ///         to anything other than the CLR default for the property type.
    ///     </para>
    ///     <para>
    ///         For entity types without generated keys, the state set is always <see cref="EntityState.Unchanged" />.
    ///     </para>
    ///     <para>
    ///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information.
    /// </remarks>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to begin updating.</param>
    /// <param name="shouldSwapAttached">Whether to swapped an already attached entity if found with one that was provided.</param>
    /// <returns>
    ///     The <see cref="EntityEntry{TEntity}" /> for the entity. The entry provides
    ///     access to change tracking information and operations for the entity.
    /// </returns>
    void BeginUpdate(TEntity entity, bool shouldSwapAttached = false);

    /// <summary>
    ///     <para>
    ///         Begins tracking given entities and entries reachable from the given entity using
    ///         the <see cref="EntityState.Unchanged" /> state by default, but see below for cases
    ///         when a different state will be used.
    ///     </para>
    ///     <para>
    ///         Generally, no database interaction will be performed until <see cref="SaveChangesAsync()" /> or <see cref="SaveChanges()"/> is called.
    ///     </para>
    ///     <para>
    ///         A recursive search of the navigation properties will be performed to find reachable entities
    ///         that are not already being tracked by the context. All entities found will be tracked
    ///         by the context.
    ///     </para>
    ///     <para>
    ///         For entity types with generated keys if an entity has its primary key value set
    ///         then it will be tracked in the <see cref="EntityState.Unchanged" /> state. If the primary key
    ///         value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
    ///         This helps ensure only new entities will be inserted.
    ///         An entity is considered to have its primary key value set if the primary key property is set
    ///         to anything other than the CLR default for the property type.
    ///     </para>
    ///     <para>
    ///         For entity types without generated keys, the state set is always <see cref="EntityState.Unchanged" />.
    ///     </para>
    ///     <para>
    ///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information.
    /// </remarks>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entities">The entity to begin updating.</param>
    /// <param name="shouldSwapAttached">Whether to swapped an already attached entity if found with one that was provided.</param>
    /// <returns>
    ///     The <see cref="EntityEntry{TEntity}" /> for the entity. The entry provides
    ///     access to change tracking information and operations for the entity.
    /// </returns>
    void BeginUpdateRange(IEnumerable<TEntity> entities, bool shouldSwapAttached = false);

    /// <summary>
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
    ///         they would be if <see cref="Attach(TEntity)" /> was called before calling this method.
    ///         This allows any cascading actions to be applied when <see cref="DbContext.SaveChanges()" /> is called.
    ///     </para>
    ///     <para>
    ///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information.
    ///     </para>
    /// </remarks>
    /// <param name="entity">The entity to remove.</param>
    /// <returns>
    ///     The <see cref="EntityEntry{TEntity}" /> for the entity. The entry provides
    ///     access to change tracking information and operations for the entity.
    /// </returns>
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

    /// <summary>
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
    /// <param name="entities">The entities to remove.</param>
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
    ///     <para>
    ///         Disables an entity.
    ///     </para>
    ///     <para>
    ///         Begins tracking the given entity via <see cref="BeginUpdate"/> and sets it's <see cref="IDisableableEntity.IsDisabled"/> property to <b>true</b>.
    ///     </para>
    /// </summary>
    /// <param name="entity">Entity to disable.</param>
    /// <exception cref="InvalidOperationException">Thrown when the given entity does not implement <see cref="IDisableableEntity"/>.</exception>
    void Disable(TEntity entity);

    /// <summary>
    ///     <para>
    ///         Disables an entity.
    ///     </para>
    ///     <para>
    ///         Tries to fetch the entity by the provided Id, then begins tracking the given entity via <see cref="BeginUpdate"/> and sets it's <see cref="IDisableableEntity.IsDisabled"/> property to <b>true</b>.
    ///     </para>
    /// </summary>
    /// <param name="id">Id of the entity to disable.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="NotFoundException">Thrown when entity with given Id is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the given entity does not implement <see cref="IDisableableEntity"/>.</exception>
    Task DisableAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     <para>
    ///         Disables a range of entities.
    ///     </para>
    ///     <para>
    ///         Begins tracking the given entities via <see cref="BeginUpdate"/> and sets their <see cref="IDisableableEntity.IsDisabled"/> properties to <b>true</b>.
    ///     </para>
    /// </summary>
    /// <param name="entities">Entities to disable.</param>
    /// <exception cref="InvalidOperationException">Thrown when the given entities do not implement <see cref="IDisableableEntity"/>.</exception>
    void DisableRange(IEnumerable<TEntity> entities);

    /// <summary>
    ///     <para>
    ///         Disables a range of entities.
    ///     </para>
    ///     <para>
    ///         Tries to fetch the entities by provided ids, then begins tracking the given entities via <see cref="BeginUpdate"/> and sets their <see cref="IDisableableEntity.IsDisabled"/> properties to <b>true</b>.
    ///     </para>
    /// </summary>
    /// <param name="ids">Ids of the entities to disable.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="InvalidOperationException">Thrown when the given entities do not implement <see cref="IDisableableEntity"/>.</exception>
    Task DisableRangeAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    /// <summary>
    ///     <para>
    ///         Detaches the given entity from the current context, and any other reachable entities that are
    ///         being tracked, such that changes applied to them will not be reflected in the database when <see cref="DbContext.SaveChanges()" /> is called.
    ///     </para>
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
