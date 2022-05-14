using System.Collections.Generic;

namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Evaluators;

public class GroupByEvaluator : IEvaluator, IInMemoryEvaluator, IEvaluatorBase
{
    private GroupByEvaluator()
    {
    }

    public static GroupByEvaluator Instance { get; } = new();

    public bool IsCriteriaEvaluator { get; } = false;

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        return specification.GroupByExpression is null
            ? query
            : query.GroupBy(specification.GroupByExpression).SelectMany(x => x);
    }

    public IEnumerable<T> Evaluate<T>(IEnumerable<T> query, ISpecification<T> specification) where T : class
    {
        return specification.GroupByExpression is null
            ? query
            : query.GroupBy(specification.GroupByExpression.Compile()).SelectMany(x => x);
    }
}