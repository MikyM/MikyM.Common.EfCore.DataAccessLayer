using EFCoreSecondLevelCacheInterceptor;

namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Evaluators;

public class CachingEvaluator : IEvaluator, IEvaluatorBase
{
    private CachingEvaluator()
    {
    }

    public static CachingEvaluator Instance { get; } = new();

    public bool IsCriteriaEvaluator { get; } = false;

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (!specification.IsCacheEnabled.HasValue) return query;
            
        return !specification.IsCacheEnabled.Value ? query.NotCacheable() : query.Cacheable();
    }
}