using MikyM.Common.EfCore.DataAccessLayer.Specifications.Extensions;

namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Validators;

public class SearchValidator : IValidator
{
    private SearchValidator() { }
    public static SearchValidator Instance { get; } = new();

    public bool IsValid<T>(T entity, ISpecification<T> specification) where T : class
    {
        if (specification.SearchCriterias is null) return true;

        foreach (var searchGroup in specification.SearchCriterias.GroupBy(x => x.SearchGroup))
        {
            if (searchGroup.Any(c => c.SelectorFunc(entity).Like(c.SearchTerm)) == false) return false;
        }

        return true;
    }
}