using MikyM.Common.DataAccessLayer;
using MikyM.Common.EfCore.DataAccessLayer.Repositories;

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

    /// <summary>
    /// Gets a repository of a given type.
    /// </summary>
    /// <remarks>You can <b>only</b> retrieve types: <see cref="IRepository{TEntity}"/>, <see cref="IRepository{TEntity,TId}"/>, <see cref="IReadOnlyRepository{TEntity}"/> and <see cref="IReadOnlyRepository{TEntity,TId}"/>.</remarks>
    /// <typeparam name="TRepository">Type of the repository to get.</typeparam>
    /// <returns>The searched for repository.</returns>
    /// <exception cref="InvalidOperationException">Thrown when couldn't find proper type or name in cache.</exception>
    /// <exception cref="NotSupportedException">Thrown when passed type isn't equal to any of the types listed in remarks, isn't a generic type or isn't an interface.</exception>
    new TRepository GetRepository<TRepository>() where TRepository : class, IRepositoryBase;
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
