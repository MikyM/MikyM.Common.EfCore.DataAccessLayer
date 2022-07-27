using Microsoft.EntityFrameworkCore.Internal;

namespace MikyM.Common.EfCore.DataAccessLayer.Helpers;

/// <summary>
/// DbContext extensions.
/// </summary>
[PublicAPI]
public static class DbContextExtensions
{
    /// <summary>
    /// Finds a tracked entity.
    /// </summary>
    /// <remarks>Based on an internal EF Core API subjected to changes with no notice.</remarks>
    /// <param name="context">Current <see cref="DbContext"/>.</param>
    /// <param name="keyValues">Primary key values.</param>
    /// <typeparam name="TEntity">Type of the searched entity.</typeparam>
    /// <returns>A tracked entity if any.</returns>
    /// <exception cref="InvalidOperationException">Thrown when key couldn't be found.</exception>
    public static TEntity? FindTracked<TEntity>(this DbContext context, params object[] keyValues)
        where TEntity : class
    {
        var entityType = context.Model.FindEntityType(typeof(TEntity));
        var key = entityType?.FindPrimaryKey();
        var stateManager = context.GetDependencies().StateManager;
        var entry = stateManager.TryGetEntry(key ?? throw new InvalidOperationException(), keyValues);
        return entry?.Entity as TEntity;
    }
}
