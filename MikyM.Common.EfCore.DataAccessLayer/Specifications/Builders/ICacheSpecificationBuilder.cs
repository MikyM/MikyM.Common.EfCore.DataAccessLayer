namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Builders;

public interface ICacheSpecificationBuilder<T> : ISpecificationBuilder<T> where T : class
{
    bool IsChainDiscarded { get; set; }
}