using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using MikyM.Common.EfCore.DataAccessLayer.Helpers;

namespace MikyM.Common.EfCore.DataAccessLayer.Context;

/// <summary>
/// Base <see cref="DbContext"/>.
/// </summary>
/// <inheritdoc cref="DbContext"/>
public abstract class EfDbContext : DbContext, IEfDbContext
{
    /// <summary>
    /// 
    /// </summary>
    protected readonly IOptions<EfCoreDataAccessConfiguration> Config;

    /// <inheritdoc />
    protected EfDbContext(DbContextOptions options) : base(options)
    {
        Config = this.GetService<IOptions<EfCoreDataAccessConfiguration>>();
    }

    /// <inheritdoc />
    protected EfDbContext(DbContextOptions options, IOptions<EfCoreDataAccessConfiguration> config) : base(options)
    {
        Config = config;
    }

    public IQueryable<TEntity> ExecuteRawSql<TEntity>(string sql, params object[] parameters) where TEntity : class
        => Set<TEntity>().FromSqlRaw(sql, parameters);

    public int ExecuteRawSql(string sql)
        => Database.ExecuteSqlRaw(sql);

    public async Task<int> ExecuteRawSqlAsync(string sql)
        => await Database.ExecuteSqlRawAsync(sql);

    public TEntity? FindTracked<TEntity>(params object[] keyValues) where TEntity : class
        => DbContextExtensions.FindTracked<TEntity>(this, keyValues);

    protected abstract void OnBeforeSaveChanges(string? userId = null);
}
