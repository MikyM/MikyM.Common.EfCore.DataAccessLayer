namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Evaluators;

public class AsSplitQueryEvaluator : IEvaluator, IEvaluatorBase
{
    private AsSplitQueryEvaluator()
    {
    }

    public static AsSplitQueryEvaluator Instance { get; } = new();

    public bool IsCriteriaEvaluator { get; } = true;

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.IsAsSplitQuery) query = query.AsSplitQuery();

        return query;
    }
}