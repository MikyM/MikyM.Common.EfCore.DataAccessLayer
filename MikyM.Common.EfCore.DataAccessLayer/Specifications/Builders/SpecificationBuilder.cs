namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Builders;

public class SpecificationBuilder<T, TResult> : SpecificationBuilder<T>, ISpecificationBuilder<T, TResult>
    where T : class where TResult : class
{
    public SpecificationBuilder(Specification<T, TResult> specification)
        : base(specification)
    {
        Specification = specification;
    }

    public new Specification<T, TResult> Specification { get; }
}

public class SpecificationBuilder<T> : ISpecificationBuilder<T> where T : class
{
    public SpecificationBuilder(Specification<T> specification)
    {
        Specification = specification;
    }

    public Specification<T> Specification { get; }
}