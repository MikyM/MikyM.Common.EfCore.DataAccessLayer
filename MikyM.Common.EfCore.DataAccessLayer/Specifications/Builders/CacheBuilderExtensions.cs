using EFCoreSecondLevelCacheInterceptor;

namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Builders;

public static class CacheBuilderExtensions
{
    /// <summary>
    /// Specify an <see cref="CacheExpirationMode"/> for this query
    /// </summary>
    /// <returns>Current <see cref="ICacheSpecificationBuilder{T}"/> instance</returns>
    public static ICacheSpecificationBuilder<TEntity> WithExpirationMode<TEntity>(this ICacheSpecificationBuilder<TEntity> builder, CacheExpirationMode mode) where TEntity : class
    {
        builder.Specification.CacheExpirationMode = mode;

        return builder;
    }

    /// <summary>
    /// Specify a cache expiration timeout <see cref="TimeSpan"/> for this query
    /// </summary>
    /// <returns>Current <see cref="ICacheSpecificationBuilder{T}"/> instance</returns>
    public static ICacheSpecificationBuilder<TEntity> WithExpirationTimeout<TEntity>(this ICacheSpecificationBuilder<TEntity> builder, TimeSpan timeout) where TEntity : class
    {
        builder.Specification.CacheTimeout = timeout;

        return builder;
    }
}