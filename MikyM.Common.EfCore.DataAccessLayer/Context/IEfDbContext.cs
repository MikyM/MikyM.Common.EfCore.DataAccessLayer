using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace MikyM.Common.EfCore.DataAccessLayer.Context;

/// <summary>
/// Base definition of a EF database context.
/// </summary>
[PublicAPI]
public interface IEfDbContext : IDisposable, IAsyncDisposable
{
    /// <summary>
    ///     <para>
    ///         Creates a LINQ query based on a raw SQL query.
    ///     </para>
    ///     <para>
    ///         If the database provider supports composing on the supplied SQL, you can compose on top of the raw SQL query using
    ///         LINQ operators: <c>context.Blogs.FromSqlRaw("SELECT * FROM Blogs").OrderBy(b => b.Name)</c>.
    ///     </para>
    ///     <para>
    ///         As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection
    ///         attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional
    ///         arguments. Any parameter values you supply will automatically be converted to a <see cref="DbParameter" />:
    ///     </para>
    ///     <code>context.Blogs.FromSqlRaw("SELECT * FROM Blogs WHERE Name = {0}", userSuppliedSearchTerm)</code>
    ///     <para>
    ///         However, <b>never</b> pass a concatenated or interpolated string (<c>$""</c>) with non-validated user-provided values
    ///         into this method. Doing so may expose your application to SQL injection attacks. To use the interpolated string syntax,
    ///         consider using <see cref="ExecuteInterpolatedSql{TEntity}" /> to create parameters.
    ///     </para>
    ///     <para>
    ///         This overload also accepts <see cref="DbParameter" /> instances as parameter values. In addition to using positional
    ///         placeholders as above (<c>{0}</c>), you can also use named placeholders directly in the SQL query string:
    ///     </para>
    ///     <code>context.Blogs.FromSqlRaw("SELECT * FROM Blogs WHERE Name = @searchTerm", new SqlParameter("@searchTerm", userSuppliedSearchTerm))</code>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-efcore-docs-raw-sql">Executing raw SQL commands with EF Core</see>
    ///     for more information.
    /// </remarks>
    /// <typeparam name="TEntity">The type of the elements.</typeparam>
    /// <param name="sql">The raw SQL query.</param>
    /// <param name="parameters">The values to be assigned to parameters.</param>
    /// <returns>An <see cref="IQueryable{T}" /> representing the raw SQL query.</returns>
    IQueryable<TEntity> ExecuteRawSql<TEntity>(string sql, params object[] parameters) where TEntity : class;

    /// <summary>
    ///     <para>
    ///         Creates a LINQ query based on an interpolated string representing a SQL query.
    ///     </para>
    ///     <para>
    ///         If the database provider supports composing on the supplied SQL, you can compose on top of the raw SQL query using
    ///         LINQ operators:
    ///     </para>
    ///     <code>context.Blogs.FromSqlInterpolated($"SELECT * FROM Blogs").OrderBy(b => b.Name)</code>
    ///     <para>
    ///         As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection
    ///         attack. You can include interpolated parameter place holders in the SQL query string. Any interpolated parameter values
    ///         you supply will automatically be converted to a <see cref="DbParameter" />:
    ///     </para>
    ///     <code>context.Blogs.FromSqlInterpolated($"SELECT * FROM Blogs WHERE Name = {userSuppliedSearchTerm}")</code>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-efcore-docs-raw-sql">Executing raw SQL commands with EF Core</see>
    ///     for more information.
    /// </remarks>
    /// <typeparam name="TEntity">The type of the elements.</typeparam>
    /// <param name="sql">The interpolated string representing a SQL query with parameters.</param>
    /// <returns>An <see cref="IQueryable{T}" /> representing the interpolated string SQL query.</returns>
    IQueryable<TEntity> ExecuteInterpolatedSql<TEntity>(FormattableString sql) where TEntity : class;

    /// <summary>
    ///     <para>
    ///         Executes the given SQL against the database and returns the number of rows affected.
    ///     </para>
    ///     <para>
    ///         Note that this method does not start a transaction. To use this method with
    ///         a transaction, first call <see cref="BeginTransaction" /> or <see cref="O:UseTransaction" />.
    ///     </para>
    ///     <para>
    ///         Note that the current <see cref="ExecutionStrategy" /> is not used by this method
    ///         since the SQL may not be idempotent and does not run in a transaction. An <see cref="ExecutionStrategy" />
    ///         can be used explicitly, making sure to also use a transaction if the SQL is not
    ///         idempotent.
    ///     </para>
    ///     <para>
    ///         As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection
    ///         attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional
    ///         arguments. Any parameter values you supply will automatically be converted to a DbParameter:
    ///     </para>
    ///     <code>
    ///         var userSuppliedSearchTerm = ".NET";
    ///         context.Database.ExecuteSqlRaw("UPDATE Blogs SET Rank = 50 WHERE Name = {0}", userSuppliedSearchTerm);
    ///     </code>
    ///     <para>
    ///         However, <b>never</b> pass a concatenated or interpolated string (<c>$""</c>) with non-validated user-provided values
    ///         into this method. Doing so may expose your application to SQL injection attacks. To use the interpolated string syntax,
    ///         consider using <see cref="ExecuteInterpolatedSql{TEntity}" /> to create parameters.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-efcore-docs-raw-sql">Executing raw SQL commands with EF Core</see>
    ///     for more information.
    /// </remarks>
    /// <param name="sql">The SQL to execute.</param>
    /// <returns>The number of rows affected.</returns>
    int ExecuteRawSql(string sql);

    /// <summary>
    ///     <para>
    ///         Executes the given SQL against the database and returns the number of rows affected.
    ///     </para>
    ///     <para>
    ///         Note that this method does not start a transaction. To use this method with
    ///         a transaction, first call <see cref="BeginTransaction" /> or <see cref="O:UseTransaction" />.
    ///     </para>
    ///     <para>
    ///         Note that the current <see cref="ExecutionStrategy" /> is not used by this method
    ///         since the SQL may not be idempotent and does not run in a transaction. An <see cref="ExecutionStrategy" />
    ///         can be used explicitly, making sure to also use a transaction if the SQL is not
    ///         idempotent.
    ///     </para>
    ///     <para>
    ///         As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection
    ///         attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional
    ///         arguments. Any parameter values you supply will automatically be converted to a DbParameter:
    ///     </para>
    ///     <code>
    ///         var userSuppliedSearchTerm = ".NET";
    ///         context.Database.ExecuteSqlRaw("UPDATE Blogs SET Rank = 50 WHERE Name = {0}", userSuppliedSearchTerm);
    ///     </code>
    ///     <para>
    ///         However, <b>never</b> pass a concatenated or interpolated string (<c>$""</c>) with non-validated user-provided values
    ///         into this method. Doing so may expose your application to SQL injection attacks. To use the interpolated string syntax,
    ///         consider using <see cref="ExecuteInterpolatedSql{TEntity}" /> to create parameters.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-efcore-docs-raw-sql">Executing raw SQL commands with EF Core</see>
    ///     for more information.
    /// </remarks>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="parameters">Parameters to use with the SQL.</param>
    /// <returns>The number of rows affected.</returns>
    int ExecuteRawSql(string sql, params object[] parameters);

    /// <summary>
    ///     <para>
    ///         Executes the given SQL against the database and returns the number of rows affected.
    ///     </para>
    ///     <para>
    ///         Note that this method does not start a transaction. To use this method with
    ///         a transaction, first call <see cref="BeginTransaction" /> or <see cref="O:UseTransaction" />.
    ///     </para>
    ///     <para>
    ///         Note that the current <see cref="ExecutionStrategy" /> is not used by this method
    ///         since the SQL may not be idempotent and does not run in a transaction. An <see cref="ExecutionStrategy" />
    ///         can be used explicitly, making sure to also use a transaction if the SQL is not
    ///         idempotent.
    ///     </para>
    ///     <code>
    ///          var userSuppliedSearchTerm = ".NET";
    ///          context.Database.ExecuteSqlRawAsync("UPDATE Blogs SET Rank = 50 WHERE Name = {0}", userSuppliedSearchTerm);
    ///      </code>
    ///     <para>
    ///         <b>Never</b> pass a concatenated or interpolated string (<c>$""</c>) with non-validated user-provided values
    ///         into this method. Doing so may expose your application to SQL injection attacks.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-efcore-docs-raw-sql">Executing raw SQL commands with EF Core</see>
    ///     for more information.
    /// </remarks>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result is the number of rows affected.
    /// </returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task<int> ExecuteRawSqlAsync(string sql, CancellationToken cancellationToken = default);

    /// <summary>
    ///     <para>
    ///         Executes the given SQL against the database and returns the number of rows affected.
    ///     </para>
    ///     <para>
    ///         Note that this method does not start a transaction. To use this method with
    ///         a transaction, first call <see cref="BeginTransaction" /> or <see cref="O:UseTransaction" />.
    ///     </para>
    ///     <para>
    ///         Note that the current <see cref="ExecutionStrategy" /> is not used by this method
    ///         since the SQL may not be idempotent and does not run in a transaction. An <see cref="ExecutionStrategy" />
    ///         can be used explicitly, making sure to also use a transaction if the SQL is not
    ///         idempotent.
    ///     </para>
    ///     <code>
    ///          var userSuppliedSearchTerm = ".NET";
    ///          context.Database.ExecuteSqlRawAsync("UPDATE Blogs SET Rank = 50 WHERE Name = {0}", userSuppliedSearchTerm);
    ///      </code>
    ///     <para>
    ///         <b>Never</b> pass a concatenated or interpolated string (<c>$""</c>) with non-validated user-provided values
    ///         into this method. Doing so may expose your application to SQL injection attacks.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-efcore-docs-raw-sql">Executing raw SQL commands with EF Core</see>
    ///     for more information.
    /// </remarks>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="parameters">Parameters to use with the SQL.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result is the number of rows affected.
    /// </returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task<int> ExecuteRawSqlAsync(string sql, CancellationToken cancellationToken = default, params object[] parameters);

    /// <summary>
    ///     <para>
    ///         Executes the given SQL against the database and returns the number of rows affected.
    ///     </para>
    ///     <para>
    ///         Note that this method does not start a transaction. To use this method with
    ///         a transaction, first call <see cref="BeginTransaction" /> or <see cref="O:UseTransaction" />.
    ///     </para>
    ///     <para>
    ///         Note that the current <see cref="ExecutionStrategy" /> is not used by this method
    ///         since the SQL may not be idempotent and does not run in a transaction. An <see cref="ExecutionStrategy" />
    ///         can be used explicitly, making sure to also use a transaction if the SQL is not
    ///         idempotent.
    ///     </para>
    ///     <code>
    ///          var userSuppliedSearchTerm = ".NET";
    ///          context.Database.ExecuteSqlRawAsync("UPDATE Blogs SET Rank = 50 WHERE Name = {0}", userSuppliedSearchTerm);
    ///      </code>
    ///     <para>
    ///         <b>Never</b> pass a concatenated or interpolated string (<c>$""</c>) with non-validated user-provided values
    ///         into this method. Doing so may expose your application to SQL injection attacks.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-efcore-docs-raw-sql">Executing raw SQL commands with EF Core</see>
    ///     for more information.
    /// </remarks>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="parameters">Parameters to use with the SQL.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result is the number of rows affected.
    /// </returns>
    Task<int> ExecuteRawSqlAsync(string sql, params object[] parameters);

    /// <summary>
    ///     Provides access to database related information and operations for this context.
    /// </summary>
    DatabaseFacade Database { get; }

    /// <summary>
    ///     <para>
    ///         Creates a <see cref="DbSet{TEntity}" /> that can be used to query and save instances of <typeparamref name="TEntity" />.
    ///     </para>
    ///     <para>
    ///         Entity Framework Core does not support multiple parallel operations being run on the same DbContext instance. This
    ///         includes both parallel execution of async queries and any explicit concurrent use from multiple threads.
    ///         Therefore, always await async calls immediately, or use separate DbContext instances for operations that execute
    ///         in parallel. See <see href="https://aka.ms/efcore-docs-threading">Avoiding DbContext threading issues</see> for more information.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-query">Querying data with EF Core</see> and
    ///     <see href="https://aka.ms/efcore-docs-change-tracking">Changing tracking</see> for more information.
    /// </remarks>
    /// <typeparam name="TEntity">The type of entity for which a set should be returned.</typeparam>
    /// <returns>A set for the given entity type.</returns>
    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    /// <summary>
    /// Finds a tracked entity.
    /// </summary>
    /// <param name="keyValues">Keys to identify the entity.</param>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// <returns>Found entity, or null if not found.</returns>
    TEntity? FindTracked<TEntity>(params object[] keyValues) where TEntity : class;

    /// <summary>
    ///     <para>
    ///         Begins tracking the given entity and entries reachable from the given entity using
    ///         the <see cref="EntityState.Unchanged" /> state by default, but see below for cases
    ///         when a different state will be used.
    ///     </para>
    ///     <para>
    ///         Generally, no database interaction will be performed until <see cref="SaveChanges()" /> is called.
    ///     </para>
    ///     <para>
    ///         A recursive search of the navigation properties will be performed to find reachable entities
    ///         that are not already being tracked by the context. All entities found will be tracked
    ///         by the context.
    ///     </para>
    ///     <para>
    ///         For entity types with generated keys if an entity has its primary key value set
    ///         then it will be tracked in the <see cref="EntityState.Unchanged" /> state. If the primary key
    ///         value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
    ///         This helps ensure only new entities will be inserted.
    ///         An entity is considered to have its primary key value set if the primary key property is set
    ///         to anything other than the CLR default for the property type.
    ///     </para>
    ///     <para>
    ///         For entity types without generated keys, the state set is always <see cref="EntityState.Unchanged" />.
    ///     </para>
    ///     <para>
    ///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information.
    /// </remarks>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to attach.</param>
    /// <returns>
    ///     The <see cref="EntityEntry{TEntity}" /> for the entity. The entry provides
    ///     access to change tracking information and operations for the entity.
    /// </returns>
    EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    ///     <para>
    ///         Begins tracking the given entities and entries reachable from the given entities using
    ///         the <see cref="EntityState.Unchanged" /> state by default, but see below for cases
    ///         when a different state will be used.
    ///     </para>
    ///     <para>
    ///         Generally, no database interaction will be performed until <see cref="SaveChanges()" /> is called.
    ///     </para>
    ///     <para>
    ///         A recursive search of the navigation properties will be performed to find reachable entities
    ///         that are not already being tracked by the context. All entities found will be tracked
    ///         by the context.
    ///     </para>
    ///     <para>
    ///         For entity types with generated keys if an entity has its primary key value set
    ///         then it will be tracked in the <see cref="EntityState.Unchanged" /> state. If the primary key
    ///         value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
    ///         This helps ensure only new entities will be inserted.
    ///         An entity is considered to have its primary key value set if the primary key property is set
    ///         to anything other than the CLR default for the property type.
    ///     </para>
    ///     <para>
    ///         For entity types without generated keys, the state set is always <see cref="EntityState.Unchanged" />.
    ///     </para>
    ///     <para>
    ///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see>
    ///     and <see href="https://aka.ms/efcore-docs-attach-range">Using AddRange, UpdateRange, AttachRange, and RemoveRange</see>
    ///     for more information.
    /// </remarks>
    /// <param name="entities">The entities to attach.</param>
    void AttachRange(IEnumerable<object> entities);

    /// <summary>
    ///     <para>
    ///         Begins tracking the given entities and entries reachable from the given entities using
    ///         the <see cref="EntityState.Unchanged" /> state by default, but see below for cases
    ///         when a different state will be used.
    ///     </para>
    ///     <para>
    ///         Generally, no database interaction will be performed until <see cref="SaveChanges()" /> is called.
    ///     </para>
    ///     <para>
    ///         A recursive search of the navigation properties will be performed to find reachable entities
    ///         that are not already being tracked by the context. All entities found will be tracked
    ///         by the context.
    ///     </para>
    ///     <para>
    ///         For entity types with generated keys if an entity has its primary key value set
    ///         then it will be tracked in the <see cref="EntityState.Unchanged" /> state. If the primary key
    ///         value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
    ///         This helps ensure only new entities will be inserted.
    ///         An entity is considered to have its primary key value set if the primary key property is set
    ///         to anything other than the CLR default for the property type.
    ///     </para>
    ///     <para>
    ///         For entity types without generated keys, the state set is always <see cref="EntityState.Unchanged" />.
    ///     </para>
    ///     <para>
    ///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see>
    ///     and <see href="https://aka.ms/efcore-docs-attach-range">Using AddRange, UpdateRange, AttachRange, and RemoveRange</see>
    ///     for more information.
    /// </remarks>
    /// <param name="entities">The entities to attach.</param>
    void AttachRange(params object[] entities);

    /// <summary>
    ///     Gets an <see cref="EntityEntry{TEntity}" /> for the given entity. The entry provides
    ///     access to change tracking information and operations for the entity.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-entity-entries">Accessing tracked entities in EF Core</see> for more information.
    /// </remarks>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to get the entry for.</param>
    /// <returns>The entry for the given entity.</returns>
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    ///     <para>
    ///         Saves all changes made in this context to the database.
    ///     </para>
    ///     <para>
    ///         This method will automatically call <see cref="ChangeTracker.DetectChanges" /> to discover any
    ///         changes to entity instances before saving to the underlying database. This can be disabled via
    ///         <see cref="ChangeTracker.AutoDetectChangesEnabled" />.
    ///     </para>
    ///     <para>
    ///         Entity Framework Core does not support multiple parallel operations being run on the same DbContext instance. This
    ///         includes both parallel execution of async queries and any explicit concurrent use from multiple threads.
    ///         Therefore, always await async calls immediately, or use separate DbContext instances for operations that execute
    ///         in parallel. See <see href="https://aka.ms/efcore-docs-threading">Avoiding DbContext threading issues</see> for more information.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-saving-data">Saving data in EF Core</see> for more information.
    /// </remarks>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous save operation. The task result contains the
    ///     number of state entries written to the database.
    /// </returns>
    /// <exception cref="DbUpdateException">
    ///     An error is encountered while saving to the database.
    /// </exception>
    /// <exception cref="DbUpdateConcurrencyException">
    ///     A concurrency violation is encountered while saving to the database.
    ///     A concurrency violation occurs when an unexpected number of rows are affected during save.
    ///     This is usually because the data in the database has been modified since it was loaded into memory.
    /// </exception>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     <para>
    ///         Saves all changes made in this context to the database.
    ///     </para>
    ///     <para>
    ///         This method will automatically call <see cref="ChangeTracker.DetectChanges" /> to discover any
    ///         changes to entity instances before saving to the underlying database. This can be disabled via
    ///         <see cref="ChangeTracker.AutoDetectChangesEnabled" />.
    ///     </para>
    ///     <para>
    ///         Entity Framework Core does not support multiple parallel operations being run on the same DbContext instance. This
    ///         includes both parallel execution of async queries and any explicit concurrent use from multiple threads.
    ///         Therefore, always await async calls immediately, or use separate DbContext instances for operations that execute
    ///         in parallel. See <see href="https://aka.ms/efcore-docs-threading">Avoiding DbContext threading issues</see> for more information.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-saving-data">Saving data in EF Core</see> for more information.
    /// </remarks>
    /// <param name="acceptAllChangesOnSuccess">
    ///     Indicates whether <see cref="ChangeTracker.AcceptAllChanges" /> is called after the changes have
    ///     been sent successfully to the database.
    /// </param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous save operation. The task result contains the
    ///     number of state entries written to the database.
    /// </returns>
    /// <exception cref="DbUpdateException">
    ///     An error is encountered while saving to the database.
    /// </exception>
    /// <exception cref="DbUpdateConcurrencyException">
    ///     A concurrency violation is encountered while saving to the database.
    ///     A concurrency violation occurs when an unexpected number of rows are affected during save.
    ///     This is usually because the data in the database has been modified since it was loaded into memory.
    /// </exception>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

    /// <summary>
    ///     <para>
    ///         Saves all changes made in this context to the database.
    ///     </para>
    ///     <para>
    ///         This method will automatically call <see cref="ChangeTracker.DetectChanges" /> to discover any
    ///         changes to entity instances before saving to the underlying database. This can be disabled via
    ///         <see cref="ChangeTracker.AutoDetectChangesEnabled" />.
    ///     </para>
    ///     <para>
    ///         Entity Framework Core does not support multiple parallel operations being run on the same DbContext instance. This
    ///         includes both parallel execution of async queries and any explicit concurrent use from multiple threads.
    ///         Therefore, always await async calls immediately, or use separate DbContext instances for operations that execute
    ///         in parallel. See <see href="https://aka.ms/efcore-docs-threading">Avoiding DbContext threading issues</see> for more information.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-saving-data">Saving data in EF Core</see> for more information.
    /// </remarks>
    /// <returns>
    ///     The number of state entries written to the database.
    /// </returns>
    /// <exception cref="DbUpdateException">
    ///     An error is encountered while saving to the database.
    /// </exception>
    /// <exception cref="DbUpdateConcurrencyException">
    ///     A concurrency violation is encountered while saving to the database.
    ///     A concurrency violation occurs when an unexpected number of rows are affected during save.
    ///     This is usually because the data in the database has been modified since it was loaded into memory.
    /// </exception>
    int SaveChanges();

    /// <summary>
    ///     The metadata about the shape of entities, the relationships between them, and how they map to the database.
    ///     May not include all the information necessary to initialize the database.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see> for more information.
    /// </remarks>
    IModel Model { get; }

    /// <summary>
    ///     Provides access to information and operations for entity instances this context is tracking.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information.
    /// </remarks>
    ChangeTracker ChangeTracker { get; }

    /// <summary>
    ///     <para>
    ///         A unique identifier for the context instance and pool lease, if any.
    ///     </para>
    ///     <para>
    ///         This identifier is primarily intended as a correlation ID for logging and debugging such
    ///         that it is easy to identify that multiple events are using the same or different context instances.
    ///     </para>
    /// </summary>
    DbContextId ContextId { get; }

    /// <summary>
    ///     Returns the name of the table to which the entity type is mapped
    ///     or <see langword="null" /> if not mapped to a table.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to get the table name for.</typeparam>
    /// <returns>The name of the table to which the entity type is mapped.</returns>
    string? GetTableName<TEntity>() where TEntity : class;
}
