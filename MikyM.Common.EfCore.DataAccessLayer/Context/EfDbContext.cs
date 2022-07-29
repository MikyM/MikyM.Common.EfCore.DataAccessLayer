using System.Collections.Generic;
using System.Threading;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using MikyM.Common.EfCore.DataAccessLayer.Helpers;

namespace MikyM.Common.EfCore.DataAccessLayer.Context;

/// <summary>
/// Base definition of a EF database context.
/// </summary>
/// <inheritdoc cref="DbContext"/>
[PublicAPI]
public abstract class EfDbContext : DbContext, IEfDbContext
{
    /// <summary>
    /// Configuration.
    /// </summary>
    protected readonly IOptions<EfCoreDataAccessConfiguration> Config;

    /// <summary>
    /// Detected changes. This will be null prior to calling SaveChangesAsync.
    /// </summary>
    protected List<EntityEntry>? DetectedChanges;

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
    /// <exception cref="InvalidOperationException">Throw when couldn't find the entity type or the table name.</exception>
    public string? GetTableName<TEntity>() where TEntity : class
        => Model.FindEntityType(typeof(TEntity))?.GetTableName() ??
           throw new InvalidOperationException($"Couldn't find table name or entity type {typeof(TEntity).Name}");
    
    /// <inheritdoc cref="DbContext.SaveChangesAsync(bool,System.Threading.CancellationToken)" />
    /// <remarks>
    /// Executes <see cref="OnBeforeSaveChanges"/> if not disabled.
    /// </remarks>
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        if (!Config.Value.DisableOnBeforeSaveChanges) 
            OnBeforeSaveChanges();
        
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DbContext.SaveChangesAsync(System.Threading.CancellationToken)" />
    /// <remarks>
    /// Executes <see cref="OnBeforeSaveChanges"/> if not disabled.
    /// </remarks>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (!Config.Value.DisableOnBeforeSaveChanges) 
            OnBeforeSaveChanges();
        
        return await base.SaveChangesAsync(true, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes an action before executing SaveChanges.
    /// </summary>
    protected virtual void OnBeforeSaveChanges(List<EntityEntry>? entries = null)
    {
        if (entries is null)
        {
            ChangeTracker.DetectChanges();
            entries = ChangeTracker.Entries().ToList();
        }
        
        foreach (var entry in entries)
        {
            if (entry.Entity is Entity entity)
                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.CreatedAt = DateTime.UtcNow;
                        entry.Property("CreatedAt").IsModified = true;
                        break;
                    case EntityState.Modified:
                        entity.UpdatedAt = DateTime.UtcNow;
                        entry.Property("UpdatedAt").IsModified = true;
                        entry.Property("CreatedAt").IsModified = false;
                        break;
                }
        }
    }
}
