using System.Collections.Generic;
using MikyM.Common.DataAccessLayer.Exceptions;
using MikyM.Common.Domain.Entities;
using MikyM.Common.Domain.Entities.Base;
using MikyM.Common.EfCore.DataAccessLayer.Context;
using MikyM.Common.EfCore.DataAccessLayer.Specifications.Evaluators;
// ReSharper disable SuspiciousTypeConversion.Global

namespace MikyM.Common.EfCore.DataAccessLayer.Repositories;

/// <summary>
/// Repository.
/// </summary>
/// <inheritdoc cref="IRepository{TEntity}"/>
[PublicAPI]
public class Repository<TEntity> : ReadOnlyRepository<TEntity>, IRepository<TEntity>
    where TEntity : class, IAggregateRootEntity
{
    internal Repository(IEfDbContext context, ISpecificationEvaluator specificationEvaluator) : base(context,
        specificationEvaluator)
    {
        
    }

    /// <inheritdoc />
    public virtual void Add(TEntity entity)
        => Set.Add(entity);

    /// <inheritdoc />
    public virtual void AddRange(IEnumerable<TEntity> entities)
        => Set.AddRange(entities);



    /// <inheritdoc />
    public virtual void BeginUpdate(TEntity entity, bool shouldSwapAttached = false)
    {
        var local = Set.Local.FirstOrDefault(entry => entry.Id.Equals(entity.Id));

        if (local is not null && shouldSwapAttached)
        {
            Context.Entry(local).State = EntityState.Detached;
        }
        else if (local is not null && !shouldSwapAttached)
        {
            return;
        }

        Context.Attach(entity);
    }

    /// <inheritdoc />
    public virtual void BeginUpdateRange(IEnumerable<TEntity> entities, bool shouldSwapAttached = false)
    {
        foreach (var entity in entities)
        {
            var local = Set.Local.FirstOrDefault(entry => entry.Id.Equals(entity.Id));

            if (local is not null && shouldSwapAttached)
            {
                Context.Entry(local).State = EntityState.Detached;
            }
            else if (local is not null && !shouldSwapAttached)
            {
                return;
            }

            Context.Attach(entity);
        }
    }

    /// <inheritdoc />
    public virtual void Delete(TEntity entity)
    {
        Set.Remove(entity);
    }

    /// <inheritdoc />
    public virtual void Delete(long id)
    {
        var entity = Context.FindTracked<TEntity>(id) ?? (TEntity) Activator.CreateInstance(typeof(TEntity), id)!;
        Set.Remove(entity);
    }

    /// <inheritdoc />
    public virtual void DeleteRange(IEnumerable<TEntity> entities)
        => Set.RemoveRange(entities);

    /// <inheritdoc />
    public virtual void DeleteRange(IEnumerable<long> ids)
    {
        var entities = ids.Select(id =>
                Context.FindTracked<TEntity>(id) ?? (TEntity) Activator.CreateInstance(typeof(TEntity), id)!)
            .ToList();
        Set.RemoveRange(entities);
    }

    /// <inheritdoc />
    public virtual void Disable(TEntity entity)
    {
        if (entity is not IDisableableEntity)
            throw new InvalidOperationException("Can't disable an entity that isn't disableable.");
        
        BeginUpdate(entity);
        ((IDisableableEntity)entity).IsDisabled = true;
    }

    /// <inheritdoc />
    public virtual async Task DisableAsync(long id)
    {
        var entity = await GetAsync(id).ConfigureAwait(false);
        
        if (entity is not IDisableableEntity)
            throw new InvalidOperationException("Can't disable an entity that isn't disableable.");
        
        BeginUpdate(entity ?? throw new NotFoundException());
        ((IDisableableEntity)entity).IsDisabled = true;
    }

    /// <inheritdoc />
    public virtual void DisableRange(IEnumerable<TEntity> entities)
    {
        var list = entities.ToList();
        
        if (list.FirstOrDefault() is not IDisableableEntity)
            throw new InvalidOperationException("Can't disable an entity that isn't disableable.");
        
        BeginUpdateRange(list);
        foreach (var entity in list) 
            ((IDisableableEntity)entity).IsDisabled = true;
    }

    /// <inheritdoc />
    public virtual async Task DisableRangeAsync(IEnumerable<long> ids)
    {
        var entities = await Set
            .Join(ids, ent => ent.Id, id => id, (ent, id) => ent)
            .ToListAsync().ConfigureAwait(false);
        
        if (entities.FirstOrDefault() is not IDisableableEntity)
            throw new InvalidOperationException("Can't disable an entity that isn't disableable.");
        
        BeginUpdateRange(entities);
        entities.ForEach(x => ((IDisableableEntity)x).IsDisabled = true);
    }

    /// <inheritdoc />
    public void Detach(TEntity entity)
    {
        Context.Entry(entity).State = EntityState.Detached;
        DetachRoot(entity);
    }

    private void DetachRoot(IAggregateRootEntity entity)
    {
        foreach (var entry in Context.Entry(entity).Navigations)
        {
            switch (entry.CurrentValue)
            {
                case IEnumerable<AggregateRootEntity> navs:
                {
                    var list = navs.ToList();

                    if (list.All(x => Context.Entry(x).State is EntityState.Detached))
                        break;

                    foreach (var nav in list.Where(x => Context.Entry(x).State is not EntityState.Detached))
                    {
                        var enumerableEntry = Context.Entry(nav);

                        enumerableEntry.State = EntityState.Detached;
                        DetachRoot(nav);
                    }
                    
                    break;
                }
                case AggregateRootEntity nav:
                {
                    var singularEntry = Context.Entry(nav);
                    if (singularEntry.State is EntityState.Detached)
                        break;

                    singularEntry.State = EntityState.Detached;
                    DetachRoot(nav);
                    break;
                }
            }
        }
    }
}
