﻿using System.Collections.Generic;
using MikyM.Common.Domain.Entities;
using MikyM.Common.Domain.Entities.Base;
using MikyM.Common.EfCore.DataAccessLayer.Context;
using MikyM.Common.EfCore.DataAccessLayer.Exceptions;
using MikyM.Common.EfCore.DataAccessLayer.Specifications.Evaluators;

namespace MikyM.Common.EfCore.DataAccessLayer.Repositories;

/// <summary>
/// Repository
/// </summary>
/// <inheritdoc cref="IRepository{TEntity}"/>
public class Repository<TEntity> : ReadOnlyRepository<TEntity>, IRepository<TEntity>
    where TEntity : class, IAggregateRootEntity
{
    internal Repository(IEfDbContext context, ISpecificationEvaluator specificationEvaluator) : base(context,
        specificationEvaluator)
    {
    }

    /// <inheritdoc />
    public virtual void Add(TEntity entity)
    {
        Context.Set<TEntity>().Add(entity);
    }

    /// <inheritdoc />
    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        Context.Set<TEntity>().AddRange(entities);
    }

    /// <inheritdoc />
    public virtual void BeginUpdate(TEntity entity, bool shouldSwapAttached = false)
    {
        var local = Context.Set<TEntity>().Local.FirstOrDefault(entry => entry.Id.Equals(entity.Id));

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
            var local = Context.Set<TEntity>().Local.FirstOrDefault(entry => entry.Id.Equals(entity.Id));

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
        Context.Set<TEntity>().Remove(entity);
    }

    /// <inheritdoc />
    public virtual void Delete(long id)
    {
        var entity = Context.FindTracked<TEntity>(id) ?? (TEntity) Activator.CreateInstance(typeof(TEntity), id)!;
        Context.Set<TEntity>().Remove(entity);
    }

    /// <inheritdoc />
    public virtual void DeleteRange(IEnumerable<TEntity> entities)
    {
        Context.Set<TEntity>().RemoveRange(entities);
    }

    /// <inheritdoc />
    public virtual void DeleteRange(IEnumerable<long> ids)
    {
        var entities = ids.Select(id =>
                Context.FindTracked<TEntity>(id) ?? (TEntity) Activator.CreateInstance(typeof(TEntity), id)!)
            .ToList();
        Context.Set<TEntity>().RemoveRange(entities);
    }

    /// <inheritdoc />
    public virtual void Disable(TEntity entity)
    {
        BeginUpdate(entity);
        entity.IsDisabled = true;
    }

    /// <inheritdoc />
    public virtual async Task DisableAsync(long id)
    {
        var entity = await GetAsync(id);
        BeginUpdate(entity ?? throw new NotFoundException());
        entity.IsDisabled = true;
    }

    /// <inheritdoc />
    public virtual void DisableRange(IEnumerable<TEntity> entities)
    {
        var aggregateRootEntities = entities.ToList();
        BeginUpdateRange(aggregateRootEntities);
        foreach (var entity in aggregateRootEntities) entity.IsDisabled = true;
    }

    /// <inheritdoc />
    public virtual async Task DisableRangeAsync(IEnumerable<long> ids)
    {
        var entities = await Context.Set<TEntity>()
            .Join(ids, ent => ent.Id, id => id, (ent, id) => ent)
            .ToListAsync();
        BeginUpdateRange(entities);
        entities.ForEach(ent => ent.IsDisabled = true);
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
