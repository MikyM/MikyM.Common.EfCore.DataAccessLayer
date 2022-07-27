using Microsoft.AspNetCore.Http;
using MikyM.Common.EfCore.DataAccessLayer.Filters;

namespace MikyM.Common.EfCore.DataAccessLayer.Pagination;

/// <summary>
/// Uri service.
/// </summary>
[PublicAPI]
public interface IUriService
{
    /// <summary>
    /// Gets page uri for pagination.
    /// </summary>
    /// <param name="filter">Filter instance.</param>
    /// <param name="route">Current route.</param>
    /// <param name="queryParams">Query parameters.</param>
    /// <returns>Uri with pagination.</returns>
    public Uri GetPageUri(PaginationFilter filter, string route, IQueryCollection? queryParams = null);
}
