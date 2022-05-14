using MikyM.Common.Domain;
using MikyM.Common.Domain.Entities;

namespace MikyM.Common.EfCore.DataAccessLayer;

/// <summary>
/// Snowflake entity
/// </summary>
public class SnowflakeEntity : AggregateRootEntity
{
    /// <inheritdoc/>
    public override long Id { get; protected set; } = IdGeneratorFactory.Build().CreateId();
    
    /// <summary>
    /// Returns the Id of this entity
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => Id.ToString();
}