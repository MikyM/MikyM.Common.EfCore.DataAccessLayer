namespace MikyM.Common.EfCore.DataAccessLayer.Specifications;

/// <summary>
/// A factory of specifications.
/// </summary>
[PublicAPI]
public interface ISpecificationFactory
{
    /// <summary>
    /// Gets a specification of a given type.
    /// </summary>
    /// <typeparam name="TSpecification">Type of the specification to get.</typeparam>
    /// <returns>The specification that was searched for.</returns>
    TSpecification GetSpecification<TSpecification>() where TSpecification : ISpecification;
}
