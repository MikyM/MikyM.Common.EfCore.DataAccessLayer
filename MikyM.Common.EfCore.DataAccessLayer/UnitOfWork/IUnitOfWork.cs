using MikyM.Common.DataAccessLayer;
using MikyM.Common.EfCore.DataAccessLayer.Context;

namespace MikyM.Common.EfCore.DataAccessLayer.UnitOfWork;

/// <summary>
/// Defines an EF Unit of Work.
/// </summary>
[PublicAPI]
public interface IUnitOfWork : IUnitOfWorkBase
{
    /// <summary>
    /// Begins a transaction.
    /// </summary>
    /// <returns>Task representing the asynchronous operation.</returns>
    Task UseTransactionAsync();
    
    /// <inheritdoc cref="IUnitOfWorkBase.CommitAsync(string)"/>
    /// <returns>Number of affected rows.</returns>
    Task<int> CommitWithCountAsync(string auditUserId);

    /// <inheritdoc cref="IUnitOfWorkBase.CommitAsync()"/>
    /// <returns>Number of affected rows.</returns>
    Task<int> CommitWithCountAsync();
}

/// <inheritdoc cref="IUnitOfWork"/>
/// <summary>
/// Defines an EF Unit of Work.
/// </summary>
/// <typeparam name="TContext">Type of context to be used.</typeparam>
[PublicAPI]
public interface IUnitOfWork<out TContext> : IUnitOfWork where TContext : IEfDbContext
{
    /// <summary>
    /// Current <see cref="DbContext"/>.
    /// </summary>
    TContext Context { get; }
}
