namespace MikyM.Common.EfCore.DataAccessLayer.Specifications;

public interface ISingleResultSpecification<T> : ISpecification<T>, ISingleResultSpecification where T : class
{
}