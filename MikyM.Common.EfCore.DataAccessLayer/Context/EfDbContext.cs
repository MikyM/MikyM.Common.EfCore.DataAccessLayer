using System.Threading;
using JetBrains.Annotations;
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
    /// Configuration.
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

    /// <inheritdoc/>
    public IQueryable<TEntity> ExecuteRawSql<TEntity>(string sql, params object[] parameters) where TEntity : class
        => Set<TEntity>().FromSqlRaw(sql, parameters);
    /// <inheritdoc/>
    public IQueryable<TEntity> ExecuteInterpolatedSql<TEntity>(FormattableString sql) where TEntity : class
        => Set<TEntity>().FromSqlInterpolated(sql);
    /// <inheritdoc/>
    public int ExecuteRawSql(string sql)
        => Database.ExecuteSqlRaw(sql);
    /// <inheritdoc/>
    public int ExecuteRawSql(string sql, params object[] parameters)
        => Database.ExecuteSqlRaw(sql, parameters);
    /// <inheritdoc/>
    public async Task<int> ExecuteRawSqlAsync(string sql, CancellationToken cancellationToken = default)
        => await Database.ExecuteSqlRawAsync(sql, cancellationToken);
    /// <inheritdoc/>
    public async Task<int> ExecuteRawSqlAsync(string sql, CancellationToken cancellationToken = default, params object[] parameters)
        => await Database.ExecuteSqlRawAsync(sql, cancellationToken, parameters);
    /// <inheritdoc/>
    public async Task<int> ExecuteRawSqlAsync(string sql, params object[] parameters)
        => await Database.ExecuteSqlRawAsync(sql, parameters);
    /// <inheritdoc/>
    public TEntity? FindTracked<TEntity>(params object[] keyValues) where TEntity : class
        => DbContextExtensions.FindTracked<TEntity>(this, keyValues);

    /// <summary>
    /// Gets the table's name that is mapped to given entity type.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Throw when couldn't find the entity type or the table name</exception>
    public string? GetTableName<TEntity>() where TEntity : class
        => Model.FindEntityType(typeof(TEntity))?.GetTableName() ??
           throw new InvalidOperationException($"Couldn't find table name or entity type {typeof(TEntity).Name}");
    
    /// <summary>
    /// Executes an action before executing SaveChanges.
    /// </summary>
    /// <param name="userId">User responsible for the changes if any.</param>
    [PublicAPI]
    protected abstract void OnBeforeSaveChanges(string? userId = null);
}
