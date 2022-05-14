using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapper;
using EFCoreSecondLevelCacheInterceptor;
using MikyM.Common.EfCore.DataAccessLayer.Filters;
using MikyM.Common.EfCore.DataAccessLayer.Specifications.Expressions;

namespace MikyM.Common.EfCore.DataAccessLayer.Specifications;

/// <summary>
///     Encapsulates query logic for <typeparamref name="T" />,
///     and projects the result into <typeparamref name="TResult" />.
/// </summary>
/// <typeparam name="T">The type being queried against.</typeparam>
/// <typeparam name="TResult">The type of the result to project results to with Automapper's ProjectTo.</typeparam>
public interface ISpecification<T, TResult> : ISpecification<T> where T : class
{
    /// <summary>
    /// Automapper configuration
    /// </summary>
    MapperConfiguration? MapperConfiguration { get; } 

    /// <summary>
    /// Members to expand
    /// </summary>
    IEnumerable<Expression<Func<TResult, object>>>? MembersToExpand { get; }

    /// <summary>
    /// Members to expand
    /// </summary>
    IEnumerable<string>? StringMembersToExpand { get; }

    /// <summary>
    ///     The transform function to apply to the result of the query encapsulated by the
    ///     <see cref="ISpecification{T, TResult}" />.
    /// </summary>
    new Func<IEnumerable<TResult>, IEnumerable<TResult>>? PostProcessingAction { get; }

    /// <summary>
    /// Evaluates the given set of entities
    /// </summary>
    /// <param name="entities">Entities to evaluate</param>
    /// <returns></returns>
    new IEnumerable<TResult> Evaluate(IEnumerable<T> entities);

    /// <summary>
    /// The transform function to apply to the <typeparamref name="T"/> element.
    /// </summary>
    Expression<Func<T, TResult>>? Selector { get; }
}

/// <summary>
///     Encapsulates query logic for <typeparamref name="T" />.
/// </summary>
/// <typeparam name="T">The type being queried against.</typeparam>
public interface ISpecification<T> : ISpecification where T : class
{
    /// <summary>
    ///     Cache timeout if any.
    /// </summary>
    public TimeSpan? CacheTimeout { get; }

    /// <summary>
    ///     Cache expiration mode if any.
    /// </summary>
    public CacheExpirationMode? CacheExpirationMode { get; }

    /// <summary>
    ///     Pagination filter to apply.
    /// </summary>
    PaginationFilter? PaginationFilter { get; }

    /// <summary>
    /// The collection of filters.
    /// </summary>
    IEnumerable<WhereExpressionInfo<T>>? WhereExpressions { get; }

    /// <summary>
    ///     The collection of predicates to group by.
    /// </summary>
    Expression<Func<T, object>>? GroupByExpression { get; }

    /// <summary>
    /// The collections of functions used to determine the sorting (and subsequent sorting),
    /// to apply to the result of the query encapsulated by the <see cref="ISpecification{T}"/>.
    /// </summary>
    IEnumerable<OrderExpressionInfo<T>>? OrderExpressions { get; }

    /// <summary>
    /// The collection of <see cref="IncludeExpressionInfo"/>s describing each include expression.
    /// This information is utilized to build Include/ThenInclude functions in the query.
    /// </summary>
    IEnumerable<IncludeExpressionInfo>? IncludeExpressions { get; }

    /// <summary>
    ///     The collection of navigation properties, as strings, to include in the query.
    /// </summary>
    IEnumerable<string>? IncludeStrings { get; }

    /// <summary>
    /// The collection of 'SQL LIKE' operations.
    /// </summary>
    IEnumerable<SearchExpressionInfo<T>>? SearchCriterias { get; }

    /// <summary>
    ///     The number of elements to return.
    /// </summary>
    int? Take { get; }

    /// <summary>
    ///     The number of elements to skip before returning the remaining elements.
    /// </summary>
    int? Skip { get; }

    /// <summary>
    ///     The transform function to apply to the result of the query encapsulated by the <see cref="ISpecification{T}" />.
    /// </summary>
    Func<IEnumerable<T>, IEnumerable<T>>? PostProcessingAction { get; }

    /// <summary>
    ///     Whether or not the results should be cached, setting this will override default caching settings for this query.
    ///     Defaults to null - no override.
    /// </summary>
    bool? IsCacheEnabled { get; }

    /// <summary>
    /// Whether pagination is enabled
    /// </summary>
    bool IsPagingEnabled { get; }

    /// <summary>
    ///     Returns whether or not the change tracker will track any of the entities
    ///     that are returned. When true, if the entity instances are modified, this will not be detected
    ///     by the change tracker.
    /// </summary>
    bool IsAsNoTracking { get; }


    /// <summary>
    ///     Returns whether or not to treat this query as split query.
    ///     by the change tracker.
    /// </summary>
    /// 
    bool IsAsSplitQuery { get; }

    /// <summary>
    ///     Returns whether or not the change tracker with track the result of this query identity resolution.
    /// </summary>
    bool IsAsNoTrackingWithIdentityResolution { get; }

    /// <summary>
    /// Returns whether or not the query should ignore the defined global query filters 
    /// </summary>
    /// <remarks>
    /// for more info: https://docs.microsoft.com/en-us/ef/core/querying/filters
    /// </remarks>
    bool IgnoreQueryFilters { get; }

    /// <summary>
    /// It returns whether the given entity satisfies the conditions of the specification.
    /// </summary>
    /// <param name="entity">The entity to be validated</param>
    /// <returns></returns>
    bool IsSatisfiedBy(T entity);

    /// <summary>
    /// Evaluates the given set of entities
    /// </summary>
    /// <param name="entities">Entities to evaluate</param>
    /// <returns></returns>
    IEnumerable<T> Evaluate(IEnumerable<T> entities);
}

/// <summary>
/// Marker specification interface
/// </summary>
public interface ISpecification
{

}