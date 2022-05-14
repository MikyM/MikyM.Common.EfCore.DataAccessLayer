namespace MikyM.Common.EfCore.DataAccessLayer.Specifications;

public interface ISpecificationFactory
{
    TSpecification GetSpecification<TSpecification>() where TSpecification : ISpecification;
}