using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MikyM.Common.Domain.Entities;
using MikyM.Common.Utilities.Extensions;

namespace MikyM.Common.EfCore.DataAccessLayer;

/// <summary>
/// Audit database entry
/// </summary>
public class AuditEntry
{
    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
    }

    public EntityEntry Entry { get; }
    public string? UserId { get; set; }
    public string? TableName { get; set; }
    public Dictionary<string, object> KeyValues { get; } = new();
    public Dictionary<string, object> OldValues { get; } = new();
    public Dictionary<string, object> NewValues { get; } = new();
    public AuditType AuditType { get; set; }
    public List<string> ChangedColumns { get; } = new();

    public AuditLog ToAudit()
    {
        return new AuditLog
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
}