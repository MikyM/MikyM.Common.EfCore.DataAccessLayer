using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;

namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Utilities;

[PublicAPI]
public class SpecificationComparer<TSpecification, TEntity> : IEqualityComparer<TSpecification> where TSpecification : Specification<TEntity> where TEntity : class
{
    public static SpecificationComparer<TSpecification, TEntity> Default { get; } = new();
    
    private SpecificationComparer()
    {
    }

    public bool Equals(TSpecification? left, TSpecification? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;
        
        return GetHashCode(left) == GetHashCode(right);
    }

    public int GetHashCode(TSpecification obj)
    {
        var whereHash = 0;
        if (obj.WhereExpressions is not null)
            foreach (var exp in obj.WhereExpressions)
                whereHash = HashCode.Combine(whereHash, ExpressionEqualityComparer.Instance.GetHashCode(exp.Filter));
        var orderHash = 0;
        if (obj.OrderExpressions is not null)
            foreach (var exp in obj.OrderExpressions)
                orderHash = HashCode.Combine(orderHash, ExpressionEqualityComparer.Instance.GetHashCode(exp.KeySelector));
        var groupHash = obj.GroupByExpression is null ? 0 : ExpressionEqualityComparer.Instance.GetHashCode(obj.GroupByExpression);
        var searchHash = 0;
        if (obj.SearchCriterias is not null)
            foreach (var exp in obj.SearchCriterias)
                searchHash = HashCode.Combine(searchHash, ExpressionEqualityComparer.Instance.GetHashCode(exp.Selector));

        return HashCode.Combine(whereHash, orderHash, groupHash, searchHash, obj.Skip ?? 1, obj.Take ?? 1);
    }
}
