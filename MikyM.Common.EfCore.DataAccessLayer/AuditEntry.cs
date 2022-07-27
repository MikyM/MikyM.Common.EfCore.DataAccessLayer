using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MikyM.Common.Domain.Entities;
using MikyM.Common.Utilities.Extensions;

namespace MikyM.Common.EfCore.DataAccessLayer;

/// <summary>
/// Audit database entry.
/// </summary>
public class AuditEntry
{
    /// <summary>
    /// Base constructor.
    /// </summary>
    /// <param name="entry">Entity entry.</param>
    public AuditEntry(EntityEntry entry)
        => Entry = entry;

    /// <summary>
    /// Entity entry.
    /// </summary>
    public EntityEntry Entry { get; }
    /// <summary>
    /// User Id.
    /// </summary>
    public string? UserId { get; set; }
    /// <summary>
    /// Table name.
    /// </summary>
    public string? TableName { get; set; }
    /// <summary>
    /// Key values.
    /// </summary>
    public Dictionary<string, object> KeyValues { get; } = new();
    /// <summary>
    /// Old values.
    /// </summary>
    public Dictionary<string, object> OldValues { get; } = new();
    /// <summary>
    /// New values.
    /// </summary>
    public Dictionary<string, object> NewValues { get; } = new();
    /// <summary>
    /// Type of the audited action.
    /// </summary>
    public AuditType AuditType { get; set; }
    /// <summary>
    /// Changed columns.
    /// </summary>
    public List<string> ChangedColumns { get; } = new();

    /// <summary>
    /// Creates a new <see cref="AuditLog"/> instance.
    /// </summary>
    /// <returns>New instance of <see cref="AuditLog"/>.</returns>
    public AuditLog ToAudit()
        => new()
        {
            UserId = UserId,
            Type = AuditType.ToString().ToSnakeCase(),
            TableName = TableName,
            PrimaryKey = JsonSerializer.Serialize(KeyValues),
            OldValues = OldValues.Count is 0 ? null : JsonSerializer.Serialize(OldValues),
            NewValues = NewValues.Count is 0 ? null : JsonSerializer.Serialize(NewValues),
            AffectedColumns = ChangedColumns.Count is 0 ? null : JsonSerializer.Serialize(ChangedColumns)
        };
}
