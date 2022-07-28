using System.Collections.Generic;
using System.Linq.Expressions;
using MikyM.Common.Domain.Entities;
using MikyM.Common.Domain.Entities.Base;
using MikyM.Common.EfCore.DataAccessLayer.Context;
using MikyM.Common.EfCore.DataAccessLayer.Specifications;
using MikyM.Common.EfCore.DataAccessLayer.Specifications.Evaluators;

namespace MikyM.Common.EfCore.DataAccessLayer.Repositories;

/// <summary>
/// Read-only repository.
/// </summary>
/// <inheritdoc cref="IReadOnlyRepository{TEntity}"/>
[PublicAPI]
public class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity :  class, IAggregateRootEntity
{
    /// <inheritdoc />
    public Type EntityType => typeof(TEntity);
    
    /// <inheritdoc />
    public IEfDbContext Context { get; }
    
    /// <inheritdoc />
    public DbSet<TEntity> Set { get; }

    /// <summary>
    /// Specification evaluator.
    /// </summary>
    protected readonly ISpecificationEvaluator SpecificationEvaluator;

    /// <summary>
    /// Internal ctor.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="specificationEvaluator"></param>
    /// <exception cref="ArgumentNullException"></exception>
    internal ReadOnlyRepository(IEfDbContext context, ISpecificationEvaluator specificationEvaluator)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Set = context.Set<TEntity>();
        SpecificationEvaluator = specificationEvaluator;
    }

    /// <inheritdoc />
    public virtual async ValueTask<TEntity?> GetAsync(params object[] keyValues)
        => await Set.FindAsync(keyValues).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<TEntity?> GetSingleBySpecAsync(ISpecification<TEntity> specification)
        => await ApplySpecification(specification)
            .FirstOrDefaultAsync().ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<TProjectTo?> GetSingleBySpecAsync<TProjectTo>(
        ISpecification<TEntity, TProjectTo> specification) where TProjectTo : class
        => await ApplySpecification(specification)
            .FirstOrDefaultAsync().ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TProjectTo>> GetBySpecAsync<TProjectTo>(
        ISpecification<TEntity, TProjectTo> specification) where TProjectTo : class
    {
        var result = await ApplySpecification(specification).ToListAsync().ConfigureAwait(false);
        return specification.PostProcessingAction is null
            ? result
            : specification.PostProcessingAction(result).ToList();
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntity>> GetBySpecAsync(ISpecification<TEntity> specification)
    {
        var result = await ApplySpecification(specification)
            .ToListAsync().ConfigureAwait(false);
        return specification.PostProcessingAction is null
            ? result
            : specification.PostProcessingAction(result).ToList();
    }

    /// <inheritdoc />
    public virtual async Task<long> LongCountAsync(ISpecification<TEntity>? specification = null)
    {
        if (specification is null) 
            return await Set.LongCountAsync().ConfigureAwait(false);

        return await ApplySpecification(specification)
            .LongCountAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        => await Set.AnyAsync(predicate).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<bool> AnyAsync(ISpecification<TEntity> specification)
        => await ApplySpecification(specification).AnyAsync().ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync()
        => await Set.ToListAsync().ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TProjectTo>> GetAllAsync<TProjectTo>() where TProjectTo : class
        => await ApplySpecification(new Specification<TEntity, TProjectTo>())
            .ToListAsync().ConfigureAwait(false);

    /// <summary>
    ///     Filters the entities  of <typeparamref name="TEntity" />, to those that match the encapsulated query logic of the
    ///     <paramref name="specification" />.
    /// </summary>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <param name="evaluateCriteriaOnly">Whether to only evaluate criteria.</param>
    /// <returns>The filtered entities as an <see cref="IQueryable{T}" />.</returns>
    protected virtual IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification,
        bool evaluateCriteriaOnly = false)
        => SpecificationEvaluator.GetQuery(Set.AsQueryable(), specification,
            evaluateCriteriaOnly);

    /// <summary>
    ///     Filters all entities of <typeparamref name="TEntity" />, that matches the encapsulated query logic of the
    ///     <paramref name="specification" />, from the database.
    ///     <para>
    ///         Projects each entity into a new form, being <typeparamref name="TResult" />.
    ///     </para>
    /// </summary>
    /// <typeparam name="TResult">The type of the value returned by the projection.</typeparam>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <returns>The filtered projected entities as an <see cref="IQueryable{T}" />.</returns>
    protected virtual IQueryable<TResult> ApplySpecification<TResult>(
        ISpecification<TEntity, TResult> specification) where TResult : class
        => SpecificationEvaluator.GetQuery(Set.AsQueryable(), specification);
}
