using MikyM.Common.DataAccessLayer;
using MikyM.Common.Utilities.Results;

namespace MikyM.Common.EfCore.DataAccessLayer.UnitOfWork;

/// <summary>
/// Unit of work definition
/// </summary>
public interface IUnitOfWork : IUnitOfWorkBase
{
    /// <summary>
    /// Begins a transaction
    /// </summary>
    /// <returns>Task representing the asynchronous operation</returns>
    Task UseTransactionAsync();
    
    /// <inheritdoc cref="IUnitOfWorkBase.CommitAsync(string)"/>
    /// <returns>Number of affected rows</returns>
    Task<int> CommitWithCountAsync(string? auditUserId);

    /// <inheritdoc cref="IUnitOfWorkBase.CommitAsync()"/>
    /// <returns>Number of affected rows</returns>
    Task<int> CommitWithCountAsync();
}

/// <inheritdoc cref="IUnitOfWork"/>
/// <summary>
/// Unit of work definition
/// </summary>
/// <typeparam name="TContext">Type of context to be used</typeparam>
public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    /// <summary>
    /// Current <see cref="DbContext"/>
    /// </summary>
    TContext Context { get; }
}