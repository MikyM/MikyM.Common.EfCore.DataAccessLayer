using System.Collections.Generic;
using MikyM.Common.EfCore.DataAccessLayer.Specifications.Evaluators;

namespace MikyM.Common.EfCore.DataAccessLayer.Specifications;

/// <inheritdoc cref="ISpecificationEvaluator" />
public class SpecificationEvaluator : ISpecificationEvaluator
{
    // Will use singleton for default configuration. Yet, it can be instantiated if necessary, with default or provided evaluators.
    /// <summary>
    /// <see cref="SpecificationEvaluator" /> instance with default evaluators and without any additional features enabled.
    /// </summary>
    public static SpecificationEvaluator Default { get; } = new();

    /// <summary>
    /// <see cref="SpecificationEvaluator" /> instance with default evaluators and enabled caching.
    /// </summary>
    public static SpecificationEvaluator Cached { get; } = new(true);

    private readonly List<IEvaluator> _evaluators = new();

    private readonly IProjectionEvaluator _projectionEvaluator;

    private SpecificationEvaluator(IEnumerable<IEvaluator> evaluators, IProjectionEvaluator projectionEvaluator)
    {
        _projectionEvaluator = projectionEvaluator;
        _evaluators.AddRange(evaluators);
    }

    private SpecificationEvaluator(bool cacheEnabled = false)
    {
        _projectionEvaluator = ProjectionEvaluator.Instance;
        _evaluators.AddRange(new List<IEvaluator>()
        {
            WhereEvaluator.Instance, SearchEvaluator.Instance, cacheEnabled ? IncludeEvaluator.Cached : IncludeEvaluator.Default,
            OrderEvaluator.Instance, PaginationEvaluator.Instance, AsNoTrackingEvaluator.Instance,
            AsSplitQueryEvaluator.Instance, AsNoTrackingWithIdentityResolutionEvaluator.Instance,
            GroupByEvaluator.Instance, CachingEvaluator.Instance
        });
    }

    public virtual IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> query,
        ISpecification<T, TResult> specification) where T : class where TResult : class
    {
        if (specification is null) throw new ArgumentNullException(nameof(specification), "Specification is required");

        query = GetQuery(query, (ISpecification<T>)specification);

        return specification.Selector is not null
            ? query.Select(specification.Selector)
            : _projectionEvaluator.GetQuery(query, specification);
    }

    public virtual IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification,
        bool evaluateCriteriaOnly = false) where T : class
    {
        if (specification is null) throw new ArgumentNullException(nameof(specification), "Specification is required");

        return (evaluateCriteriaOnly ? _evaluators.Where(x => x.IsCriteriaEvaluator) : _evaluators).Aggregate(query,
            (current, evaluator) => evaluator.GetQuery(current, specification));
    }
}