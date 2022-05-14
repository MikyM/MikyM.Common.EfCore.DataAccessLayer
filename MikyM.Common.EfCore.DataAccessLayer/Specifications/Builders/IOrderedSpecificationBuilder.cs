namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Builders;

public interface IOrderedSpecificationBuilder<T> : ISpecificationBuilder<T> where T : class
{
    bool IsChainDiscarded { get; set; }
}