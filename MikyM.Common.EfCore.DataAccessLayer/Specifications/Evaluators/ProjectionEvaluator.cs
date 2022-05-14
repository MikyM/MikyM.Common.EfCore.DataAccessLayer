using AutoMapper.QueryableExtensions;

namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Evaluators;

public class ProjectionEvaluator : IProjectionEvaluator, IEvaluatorBase
{
    public static ProjectionEvaluator Instance { get; } = new();

    private ProjectionEvaluator()
    {
        
    }

    public IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> query, ISpecification<T, TResult> specification) where T : class where TResult : class
    {
        if (specification.MembersToExpand is not null)
        {
            return specification.MapperConfiguration is null
                ? query.ProjectTo(specification.MembersToExpand.ToArray())
                : query.ProjectTo(specification.MapperConfiguration,
                    specification.MembersToExpand.ToArray());
        }

        if (specification.StringMembersToExpand is not null)
        {
            return specification.MapperConfiguration is null
                ? query.ProjectTo<TResult>(null, specification.StringMembersToExpand.ToArray())
                : query.ProjectTo<TResult>(specification.MapperConfiguration, null,
                    specification.StringMembersToExpand.ToArray());
        }

        return specification.MapperConfiguration is not null
            ? query.ProjectTo<TResult>(specification.MapperConfiguration)
            : query.ProjectTo<TResult>();
    }
}