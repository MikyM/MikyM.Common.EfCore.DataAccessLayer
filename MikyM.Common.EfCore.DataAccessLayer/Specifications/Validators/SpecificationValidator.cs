using System.Collections.Generic;

namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Validators;

public class SpecificationValidator : ISpecificationValidator
{
    // Will use singleton for default configuration. Yet, it can be instantiated if necessary, with default or provided validators.
    public static SpecificationValidator Default { get; } = new();

    private readonly List<IValidator> _validators = new();

    private SpecificationValidator()
    {
        _validators.AddRange(new IValidator[]
        {
            WhereValidator.Instance,
            SearchValidator.Instance
        });
    }
    private SpecificationValidator(IEnumerable<IValidator> validators)
    {
        _validators.AddRange(validators);
    }

    public virtual bool IsValid<T>(T entity, ISpecification<T> specification) where T : class
    {
        foreach (var partialValidator in _validators)
        {
            if (partialValidator.IsValid(entity, specification) == false) return false;
        }

        return true;
    }
}