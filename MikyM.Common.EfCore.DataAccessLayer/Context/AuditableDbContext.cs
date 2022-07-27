using System.Collections.Generic;
using System.Threading;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using MikyM.Common.Domain.Entities;

namespace MikyM.Common.EfCore.DataAccessLayer.Context;

/// <summary>
/// Auditable <see cref="DbContext"/>.
/// </summary>
/// <inheritdoc cref="DbContext"/>
[PublicAPI]
public abstract class AuditableDbContext : EfDbContext
{
    /// <summary>
    /// Id of the user responsible for changes done within this context.
    /// </summary>
    protected string? AuditUserId { get; set; }

    /// <inheritdoc />
    protected AuditableDbContext(DbContextOptions options) : base(options)
    {
    }

    /// <inheritdoc />
    protected AuditableDbContext(DbContextOptions options, IOptions<EfCoreDataAccessConfiguration> config) : base(options)
    {
    }

    // ReSharper disable once MemberCanBePrivate.Global
    /// <summary>
    /// Audit log <see cref="DbSet{TEntity}"/>.
    /// </summary>
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;


    /// <summary>
    /// Prior to calling base SaveChanges creates an audit log entry with user Id information.
    /// </summary>
    /// <param name="auditUserId">Id of the user responsible for the change.</param>
    /// <param name="acceptAllChangesOnSuccess">Whether to accept all changes on success.</param>
    /// <param name="cancellationToken">A cancellation token if any.</param>
    /// <returns>Number of affected entries.</returns>
    public virtual async Task<int> SaveChangesAsync(string auditUserId, bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        AuditUserId = auditUserId;
        return await SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Prior to calling base SaveChanges creates an audit log entry with user Id information.
    /// </summary>
    /// <param name="auditUserId">Id of the user responsible for the change.</param>
    /// <param name="cancellationToken">A cancellation token if any.</param>
    /// <returns>Number of affected entries.</returns>
    public virtual async Task<int> SaveChangesAsync(string auditUserId, CancellationToken cancellationToken = default)
    {
        AuditUserId = auditUserId;
        return await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }


    /// <inheritdoc/>
    /// <remarks>Handles audit logs and <see cref="Entity.CreatedAt"/>, <see cref="Entity.UpdatedAt"/> properties.</remarks>
    protected override void OnBeforeSaveChanges(List<EntityEntry>? detectedEntries = null)
    {
        ChangeTracker.DetectChanges();
        var detectedChanges = ChangeTracker.Entries().ToList();
        
        var auditEntries = new List<AuditEntry>();
        foreach (var entry in detectedChanges)
        {
            if (entry.Entity is AuditLog || entry.State is EntityState.Detached or EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry) { TableName = entry.Entity.GetType().Name, UserId = AuditUserId };

            auditEntries.Add(auditEntry);

            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue!;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = AuditType.Create;
                        auditEntry.NewValues[propertyName] = property.CurrentValue!;
                        break;
                    case EntityState.Deleted:
                        auditEntry.AuditType = AuditType.Disable;
                        auditEntry.OldValues[propertyName] = property.OriginalValue!;
                        break;
                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.ChangedColumns.Add(propertyName);
                            auditEntry.AuditType = AuditType.Update;
                            if (entry.Entity is Entity && propertyName == "IsDisabled" && property.IsModified &&
                                !(bool)property.OriginalValue! &&
                                (bool)property.CurrentValue!) auditEntry.AuditType = AuditType.Disable;
                            auditEntry.OldValues[propertyName] = property.OriginalValue!;
                            auditEntry.NewValues[propertyName] = property.CurrentValue!;
                        }

                        break;
                }
            }
        }

        foreach (var auditEntry in auditEntries) 
            AuditLogs.Add(auditEntry.ToAudit());
        
        base.OnBeforeSaveChanges(detectedChanges);
    }
}
