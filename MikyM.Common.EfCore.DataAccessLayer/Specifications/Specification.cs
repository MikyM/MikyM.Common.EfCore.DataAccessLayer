using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapper;
using EFCoreSecondLevelCacheInterceptor;
using MikyM.Common.EfCore.DataAccessLayer.Filters;
using MikyM.Common.EfCore.DataAccessLayer.Specifications.Builders;
using MikyM.Common.EfCore.DataAccessLayer.Specifications.Evaluators;
using MikyM.Common.EfCore.DataAccessLayer.Specifications.Expressions;
using MikyM.Common.EfCore.DataAccessLayer.Specifications.Validators;

namespace MikyM.Common.EfCore.DataAccessLayer.Specifications;

/// <inheritdoc cref="ISpecification{T,TResult}" />
public
    class Specification<T, TResult> : Specification<T>, ISpecification<T, TResult>
    where T : class where TResult : class
{
    protected internal Specification() : this(InMemorySpecificationEvaluator.Default)
    {
    }

    public Specification(PaginationFilter paginationFilter) : this(InMemorySpecificationEvaluator.Default, paginationFilter)
    {
    }

    protected Specification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator, PaginationFilter paginationFilter) : base(
        inMemorySpecificationEvaluator, paginationFilter)
    {
        Query = new SpecificationBuilder<T, TResult>(this);
    }

    protected Specification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator) : base(
        inMemorySpecificationEvaluator)
    {
        Query = new SpecificationBuilder<T, TResult>(this);
    }

    /// <summary>
    /// Inner <see cref="ISpecificationBuilder{T,TResult}"/>
    /// </summary>
    protected new virtual ISpecificationBuilder<T, TResult> Query { get; }

    /// <inheritdoc />
    public new virtual IEnumerable<TResult> Evaluate(IEnumerable<T> entities)
    {
        return Evaluator.Evaluate(entities, this);
    }

    /// <inheritdoc/>
    public Expression<Func<T, TResult>>? Selector { get; internal set; }

    /// <inheritdoc />
    public MapperConfiguration? MapperConfiguration { get; internal set; }

    /// <inheritdoc />
    public IEnumerable<Expression<Func<TResult, object>>>? MembersToExpand { get; internal set; }

    /// <inheritdoc />
    public IEnumerable<string>? StringMembersToExpand { get; internal set; }

    /// <inheritdoc />
    public new Func<IEnumerable<TResult>, IEnumerable<TResult>>? PostProcessingAction { get; internal set; }

    /// <summary>
    /// Specify a transform function to apply to the result of the query.
    /// and returns another <typeparamref name="TResult"/> type
    /// </summary>
    protected Specification<T> WithPostProcessingAction(Func<IEnumerable<TResult>, IEnumerable<TResult>> postProcessingAction)
    {
        Query.WithPostProcessingAction(postProcessingAction);
        return this;
    }

    /// <summary>
    /// Adds automapper configuration.
    /// </summary>
    /// <param name="mapperConfiguration"><see cref="MapperConfiguration"/> instance</param>
    /// <returns>Current specification instance</returns>
    protected Specification<T> WithMapperConfiguration(MapperConfiguration mapperConfiguration)
    {
        Query.WithMapperConfiguration(mapperConfiguration);
        return this;
    }
    
    /// <summary>
    /// Specify a transform function to apply to the <typeparamref name="T"/> element 
    /// </summary>
    /// <param name="selector">Selector</param>
    /// <returns>Current specification instance</returns>
    protected Specification<T> Select(Expression<Func<T, TResult>> selector)
    {
        Query.Select(selector);
        return this;
    }

    /// <summary>
    /// Expands given member.
    /// </summary>
    /// <param name="expression">Member to expand</param>
    /// <returns>Current specification instance</returns>
    protected Specification<T> Expand(Expression<Func<TResult, object>> expression)
    {
        Query.Expand(expression);
        return this;
    }

    /// <summary>
    /// Expands given member.
    /// </summary>
    /// <param name="member">Member to expand</param>
    /// <returns>Current specification instance</returns>
    protected Specification<T> Expand(string member)
    {
        Query.Expand(member);
        return this;
    }
}

/// <inheritdoc cref="ISpecification{T}" />
public class Specification<T> : ISpecification<T> where T : class
{
    protected Specification(PaginationFilter paginationFilter)
        : this(InMemorySpecificationEvaluator.Default, SpecificationValidator.Default, paginationFilter)
    {
    }

    protected Specification()
        : this(InMemorySpecificationEvaluator.Default, SpecificationValidator.Default)
    {
    }

    protected Specification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator)
        : this(inMemorySpecificationEvaluator, SpecificationValidator.Default)
    {
    }

    protected Specification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator, PaginationFilter paginationFilter)
        : this(inMemorySpecificationEvaluator, SpecificationValidator.Default, paginationFilter)
    {
    }

    protected Specification(ISpecificationValidator specificationValidator, PaginationFilter paginationFilter)
        : this(InMemorySpecificationEvaluator.Default, specificationValidator, paginationFilter)
    {
    }

    protected Specification(ISpecificationValidator specificationValidator)
        : this(InMemorySpecificationEvaluator.Default, specificationValidator)
    {
    }

    protected Specification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator, ISpecificationValidator specificationValidator)
    {
        Evaluator = inMemorySpecificationEvaluator;
        Validator = specificationValidator;
        Query = new SpecificationBuilder<T>(this);
    }

    protected Specification(IInMemorySpecificationEvaluator inMemorySpecificationEvaluator, ISpecificationValidator specificationValidator, PaginationFilter paginationFilter)
    {
        Evaluator = inMemorySpecificationEvaluator;
        Validator = specificationValidator;
        Query = new SpecificationBuilder<T>(this);
        PaginationFilter = paginationFilter;
    }

    /// <summary>
    /// Inner <see cref="InMemorySpecificationEvaluator"/>
    /// </summary>
    protected IInMemorySpecificationEvaluator Evaluator { get; }
    /// <summary>
    /// Innner <see cref="ISpecificationEvaluator"/>
    /// </summary>
    protected ISpecificationValidator Validator { get; }
    /// <summary>
    /// Inner <see cref="ISpecificationBuilder{T}"/>
    /// </summary>
    protected virtual ISpecificationBuilder<T> Query { get; }

    /// <summary>
    /// Evaluates given <see cref="IEnumerable{T}"/> using self.
    /// </summary>
    /// <param name="entities">Entities to evaluate</param>
    /// <returns></returns>
    public virtual IEnumerable<T> Evaluate(IEnumerable<T> entities)
    {
        return Evaluator.Evaluate(entities, this);
    }

    /// <inheritdoc/>
    public virtual bool IsSatisfiedBy(T entity)
    {
        return Validator.IsValid(entity, this);
    }

    /// <inheritdoc />
    public TimeSpan? CacheTimeout { get; internal set; }

    /// <inheritdoc />
    public CacheExpirationMode? CacheExpirationMode { get; internal set; }

    /// <inheritdoc />
    public IEnumerable<WhereExpressionInfo<T>>? WhereExpressions { get; internal set; }

    /// <inheritdoc />
    public IEnumerable<OrderExpressionInfo<T>>? OrderExpressions
    {
        get;
        internal set;
    }

    /// <inheritdoc />
    public IEnumerable<IncludeExpressionInfo>? IncludeExpressions { get; internal set; }

    /// <inheritdoc />
    public Expression<Func<T, object>>? GroupByExpression { get; internal set; }

    /// <inheritdoc />
    public IEnumerable<string>? IncludeStrings { get; internal set; }

    /// <inheritdoc />
    public IEnumerable<SearchExpressionInfo<T>>? SearchCriterias
    {
        get;
        internal set;
    }

    /// <inheritdoc/>
    public bool IgnoreQueryFilters { get; internal set; } = false;

    /// <inheritdoc />
    public int? Take { get; internal set; }

    /// <inheritdoc />
    public int? Skip { get; internal set; }

    private PaginationFilter? _paginationFilter;

    /// <inheritdoc />
    public PaginationFilter? PaginationFilter
    {
        get
        {
            if (_paginationFilter is not null) return _paginationFilter;
            if (!Take.HasValue || !Skip.HasValue) return null;

            _paginationFilter = new PaginationFilter(Skip.Value / Take.Value + 1, Take.Value);
            if (!IsPagingEnabled) IsPagingEnabled = true;

            return _paginationFilter;

        }
        internal set
        {
            if (value is null)
            {
                _paginationFilter = value;
                return;
            }

            _paginationFilter = value;
            Skip = (value.PageNumber - 1) * value.PageSize;
            Take = value.PageSize;

            IsPagingEnabled = true;
        }
    }

    /// <inheritdoc />
    public Func<IEnumerable<T>, IEnumerable<T>>? PostProcessingAction { get; internal set; }

    /// <inheritdoc />
    public bool? IsCacheEnabled { get; internal set; }

    /// <inheritdoc />
    public bool IsPagingEnabled { get; internal set; }

    /// <inheritdoc />
    public bool IsAsNoTracking { get; internal set; } = true;

    /// <inheritdoc />
    public bool IsAsSplitQuery { get; internal set; }

    /// <inheritdoc />
    public bool IsAsNoTrackingWithIdentityResolution { get; internal set; }

    /// <summary>
    /// Sets whether to ignore query filters or not, default is
    /// </summary>
    /// <param name="ignore">Whether to ignore query filters</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T,TResult}"/> instance</returns>
    protected ISpecificationBuilder<T> WithIgnoreQueryFilters(bool ignore = true)
    {
        return Query.IgnoreQueryFilters(ignore);
    }

    /// <summary>
    /// Specify a transform function to apply to the result of the query 
    /// and returns the same <typeparamref name="T"/> type
    /// </summary>
    /// <param name="postProcessingAction">Action to use for post processing</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected ISpecificationBuilder<T> WithPostProcessingAction(Func<IEnumerable<T>, IEnumerable<T>> postProcessingAction)
    {
        return Query.WithPostProcessingAction(postProcessingAction);
    }

    /// <summary>
    /// Specify an include expression.
    /// This information is utilized to build Include function in the query, which ORM tools like Entity Framework use
    /// to include related entities (via navigation properties) in the query result.
    /// </summary>
    /// <param name="includeExpression">Member to include</param>
    /// <returns><see cref="IIncludableSpecificationBuilder{T,TProperty}"/> instance</returns>
    protected IIncludableSpecificationBuilder<T, TProperty?> Include<TProperty>(
        Expression<Func<T, TProperty?>> includeExpression)
    {
        return Query.Include(includeExpression);
    }

    /// <summary>
    /// Specify the query result will be ordered by <paramref name="orderByExpression"/> in an ascending order
    /// </summary>
    /// <param name="orderByExpression">Member to use for ordering</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected ISpecificationBuilder<T> OrderBy(Expression<Func<T, object?>> orderByExpression)
    {
        return Query.OrderBy(orderByExpression);
    }

    /// <summary>
    /// Specify the query result will be ordered by <paramref name="orderByDescendingExpression"/> in a descending order
    /// </summary>
    /// <param name="orderByDescendingExpression">Member to use for ordering</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected ISpecificationBuilder<T> OrderByDescending(Expression<Func<T, object?>> orderByDescendingExpression)
    {
        return Query.OrderByDescending(orderByDescendingExpression);
    }

    /// <summary>
    /// Specify a predicate that will be applied to the query
    /// </summary>
    /// <param name="criteria">Given criteria</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected ISpecificationBuilder<T> Where(Expression<Func<T, bool>> criteria)
    {
        return Query.Where(criteria);
    }

    /// <summary>
    /// Specify the query result will be grouped by <paramref name="groupByExpression"/> in a descending order
    /// </summary>
    /// <param name="groupByExpression">Member to use for grouping</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected ISpecificationBuilder<T> GroupBy(Expression<Func<T, object>> groupByExpression)
    {
        return Query.GroupBy(groupByExpression);
    }

    /// <summary>
    /// Specify a 'SQL LIKE' operations for search purposes
    /// </summary>
    /// <param name="selector">the property to apply the SQL LIKE against</param>
    /// <param name="searchTerm">the value to use for the SQL LIKE</param>
    /// <param name="searchGroup">the index used to group sets of Selectors and SearchTerms together</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected ISpecificationBuilder<T> Search(Expression<Func<T, string>> selector, string searchTerm, int searchGroup = 1)
    {
        return Query.Search(selector, searchTerm, searchGroup);
    }

    /// <summary>
    /// Specify a <see cref="PaginationFilter"/> to use
    /// </summary>
    /// <param name="paginationFilter"><see cref="PaginationFilter"/> to use</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected ISpecificationBuilder<T> WithPaginationFilter(PaginationFilter paginationFilter)
    {
        return Query.WithPaginationFilter(paginationFilter);
    }

    /// <summary>
    /// Specify the number of elements to return.
    /// </summary>
    /// <param name="take">number of elements to take</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected ISpecificationBuilder<T> ApplyTake(int take)
    {
        return Query.Take(take);
    }

    /// <summary>
    /// Specify the number of elements to skip before returning the remaining elements.
    /// </summary>
    /// <param name="skip">number of elements to skip</param>
    /// <returns>Current <see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected ISpecificationBuilder<T> ApplySkip(int skip)
    {
        return Query.Skip(skip);
    }

    /// <summary>
    /// Specify whether results should be cached using <see cref="SecondLevelCacheInterceptor"/>.
    /// </summary>
    /// <param name="withCaching">Whether to cache results</param>
    /// <returns><see cref="ICacheSpecificationBuilder{T}"/> instance</returns>
    protected ICacheSpecificationBuilder<T> WithCaching(bool withCaching = true)
    {
        return Query.WithCaching(withCaching); 
    }

    /// <summary>
    /// Specify whether to use tracking for this query (default setting is AsNoTracking)
    /// </summary>
    /// <returns><see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected ISpecificationBuilder<T> AsTracking()
    {
        return Query.AsTracking();
    }

    /// <summary>
    /// Specify whether to use tracking use split query
    /// </summary>
    /// <returns><see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected ISpecificationBuilder<T> AsSplitQuery()
    {
        return Query.AsSplitQuery();
    }

    /// <summary>
    /// Specify whether to use AsNoTrackingWithIdentityResolution
    /// </summary>
    /// <returns><see cref="ISpecificationBuilder{T}"/> instance</returns>
    protected ISpecificationBuilder<T> AsNoTrackingWithIdentityResolution()
    {
        return Query.AsNoTrackingWithIdentityResolution();
    }
}