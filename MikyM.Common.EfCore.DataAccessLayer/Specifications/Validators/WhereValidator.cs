namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Validators;

public class WhereValidator : IValidator
{
    private WhereValidator() { }
    public static WhereValidator Instance { get; } = new();

    public bool IsValid<T>(T entity, ISpecification<T> specification) where T : class
    {
        if (specification.WhereExpressions is null) return true;

        foreach (var info in specification.WhereExpressions)
        {
            if (info.FilterFunc(entity) == false) return false;
        }

        return true;
    }
}