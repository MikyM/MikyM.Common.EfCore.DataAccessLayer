using MikyM.Common.DataAccessLayer.Repositories;
using MikyM.Common.EfCore.DataAccessLayer.Repositories;

namespace MikyM.Common.EfCore.DataAccessLayer.UnitOfWork;

/// <summary>
/// Unit of work definition
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Gets a repository of a given type
    /// </summary>
    /// <typeparam name="TRepository">Type of the repository to get</typeparam>
    /// <returns>Wanted repository</returns>
    TRepository GetRepository<TRepository>() where TRepository : class, IBaseRepository;
    /// <summary>
    /// Commits changes
    /// </summary>
    /// <returns>Number of affected rows</returns>
    Task<int> CommitAsync();
    /// <summary>
    /// Commits changes
    /// </summary>
    /// <param name="userId">Id of the user that is responsible for doing changes</param>
    /// <returns>Number of affected rows</returns>
    Task<int> CommitAsync(string? userId);
    /// <summary>
    /// Rolls the transaction back
    /// </summary>
    /// <returns>Task representing the asynchronous operation</returns>
    Task RollbackAsync();
    /// <summary>
    /// Begins a transaction
    /// </summary>
    /// <returns>Task representing the asynchronous operation</returns>
    Task UseTransactionAsync();
}

/// <inheritdoc cref="IUnitOfWork"/>
/// <summary>
/// Unit of work definition
/// </summary>
/// <typeparam name="TContext">Type of context to be used</typeparam>
public interface IUnitOfWork<TContext> : IUnitOfWork, IDisposable where TContext : DbContext
{
    /// <summary>
    /// Current <see cref="DbContext"/>
    /// </summary>
    TContext Context { get; }
}