using System.Collections.Generic;
using System.Linq.Expressions;
using MikyM.Common.EfCore.DataAccessLayer.Specifications.Expressions;

namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Builders;

public static class IncludableBuilderExtensions
{
    /// <summary>
    /// Specify a nested member to include
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPreviousProperty"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="previousBuilder"></param>
    /// <param name="thenIncludeExpression">Nested member to include</param>
    /// <returns>Current <see cref="IIncludableSpecificationBuilder{T,TProperty}"/> instance</returns>
    public static IIncludableSpecificationBuilder<TEntity, TProperty?> ThenInclude<TEntity, TPreviousProperty, TProperty>(
        this IIncludableSpecificationBuilder<TEntity, TPreviousProperty?> previousBuilder,
        Expression<Func<TPreviousProperty?, TProperty?>> thenIncludeExpression)
        where TEntity : class
        => ThenInclude(previousBuilder, thenIncludeExpression, true);

    /// <summary>
    /// Specify a nested member to include
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPreviousProperty"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="previousBuilder"></param>
    /// <param name="thenIncludeExpression">Nested member to include</param>
    /// <param name="condition">Condition as to when should given member be included</param>
    /// <returns>Current <see cref="IIncludableSpecificationBuilder{T,TProperty}"/> instance</returns>
    public static IIncludableSpecificationBuilder<TEntity, TProperty?> ThenInclude<TEntity, TPreviousProperty, TProperty>(
        this IIncludableSpecificationBuilder<TEntity, TPreviousProperty?> previousBuilder,
        Expression<Func<TPreviousProperty?, TProperty?>> thenIncludeExpression,
        bool condition)
        where TEntity : class
    {
        if (condition && !previousBuilder.IsChainDiscarded)
        {
            var info = new IncludeExpressionInfo(thenIncludeExpression, typeof(TEntity), typeof(TProperty), typeof(TPreviousProperty));

            previousBuilder.Specification.IncludeExpressions ??= new List<IncludeExpressionInfo>();
            ((List<IncludeExpressionInfo>)previousBuilder.Specification.IncludeExpressions).Add(info);
        }

        var includeBuilder = new IncludableSpecificationBuilder<TEntity, TProperty?>(previousBuilder.Specification, !condition || previousBuilder.IsChainDiscarded);

        return includeBuilder;
    }

    /// <summary>
    /// Specify a nested member to include
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPreviousProperty"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="previousBuilder"></param>
    /// <param name="thenIncludeExpression">Nested member to include</param>
    /// <returns>Current <see cref="IIncludableSpecificationBuilder{T,TProperty}"/> instance</returns>
    public static IIncludableSpecificationBuilder<TEntity, TProperty?> ThenInclude<TEntity, TPreviousProperty, TProperty>(
        this IIncludableSpecificationBuilder<TEntity, IEnumerable<TPreviousProperty>?> previousBuilder,
        Expression<Func<TPreviousProperty?, TProperty?>> thenIncludeExpression)
        where TEntity : class
        => ThenInclude(previousBuilder, thenIncludeExpression, true);

    /// <summary>
    /// Specify a nested member to include
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPreviousProperty"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="previousBuilder"></param>
    /// <param name="thenIncludeExpression">Nested member to include</param>
    /// <param name="condition">Condition as to when should given member be included</param>
    /// <returns>Current <see cref="IIncludableSpecificationBuilder{T,TProperty}"/> instance</returns>
    public static IIncludableSpecificationBuilder<TEntity, TProperty?> ThenInclude<TEntity, TPreviousProperty, TProperty>(
        this IIncludableSpecificationBuilder<TEntity, IEnumerable<TPreviousProperty>?> previousBuilder,
        Expression<Func<TPreviousProperty?, TProperty?>> thenIncludeExpression,
        bool condition)
        where TEntity : class
    {
        if (condition && !previousBuilder.IsChainDiscarded)
        {
            var info = new IncludeExpressionInfo(thenIncludeExpression, typeof(TEntity), typeof(TProperty), typeof(IEnumerable<TPreviousProperty>));

            previousBuilder.Specification.IncludeExpressions ??= new List<IncludeExpressionInfo>();
            ((List<IncludeExpressionInfo>)previousBuilder.Specification.IncludeExpressions).Add(info);
        }

        var includeBuilder = new IncludableSpecificationBuilder<TEntity, TProperty?>(previousBuilder.Specification, !condition || previousBuilder.IsChainDiscarded);

        return includeBuilder;
    }
}