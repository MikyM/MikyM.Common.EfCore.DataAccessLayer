using System.Collections.Generic;
using System.Threading;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MikyM.Common.EfCore.DataAccessLayer.Context;

/// <summary>
/// Base EF Context interface.
/// </summary>
public interface IEfDbContext : IDisposable, IAsyncDisposable
{
    IQueryable<TEntity> ExecuteRawSql<TEntity>(string sql, params object[] parameters) where TEntity : class;
    int ExecuteRawSql(string sql);
    Task<int> ExecuteRawSqlAsync(string sql);
    DatabaseFacade Database { get; }
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    TEntity? FindTracked<TEntity>(params object[] keyValues) where TEntity : class;
    EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;
    void AttachRange(IEnumerable<object> entity);
    void AttachRange(params object[] entities);
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
    int SaveChanges();
    IModel Model { get; }
    ChangeTracker ChangeTracker { get; }
    DbContextId ContextId { get; }
    string? GetTableName<TEntity>() where TEntity : class;
}
