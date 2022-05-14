namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Evaluators;

public interface IEvaluator
{
    bool IsCriteriaEvaluator { get; }

    IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class;
}