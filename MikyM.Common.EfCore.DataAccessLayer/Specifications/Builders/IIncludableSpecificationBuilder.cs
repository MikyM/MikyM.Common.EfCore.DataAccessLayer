namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Builders;

public interface IIncludableSpecificationBuilder<T, out TProperty> : ISpecificationBuilder<T> where T : class
{
    bool IsChainDiscarded { get; set; }
}