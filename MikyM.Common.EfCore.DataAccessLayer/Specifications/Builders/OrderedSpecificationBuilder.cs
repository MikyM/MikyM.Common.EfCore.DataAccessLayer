namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Builders;

public class OrderedSpecificationBuilder<T> : IOrderedSpecificationBuilder<T> where T : class
{
    public Specification<T> Specification { get; }
    public bool IsChainDiscarded { get; set; }

    public OrderedSpecificationBuilder(Specification<T> specification, bool isChainDiscarded = false)
    {
        Specification = specification;
        IsChainDiscarded = isChainDiscarded;
    }
}