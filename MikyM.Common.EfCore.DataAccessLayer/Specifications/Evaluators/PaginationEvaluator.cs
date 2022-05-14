using System.Collections.Generic;

namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Evaluators;

public class PaginationEvaluator : IEvaluator, IInMemoryEvaluator, IEvaluatorBase
{
    private PaginationEvaluator()
    {
    }

    public static PaginationEvaluator Instance { get; } = new();

    public bool IsCriteriaEvaluator { get; } = false;

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (!specification.IsPagingEnabled) return query;

        // If skip is 0, avoid adding to the IQueryable. It will generate more optimized SQL that way.
        if (specification.Skip is not null && specification.Skip != 0) query = query.Skip(specification.Skip.Value);

        if (specification.Take is not null) query = query.Take(specification.Take.Value);

        return query;
    }

    public IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification) where T : class
    {
        if (!specification.IsPagingEnabled) return query;

        if (specification.Skip is not null && specification.Skip != 0) query = query.Skip(specification.Skip.Value);

        if (specification.Take is not null) query = query.Take(specification.Take.Value);

        return query;
    }
}