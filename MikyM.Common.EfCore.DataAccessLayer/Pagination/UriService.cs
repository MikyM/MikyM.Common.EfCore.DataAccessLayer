using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using MikyM.Common.EfCore.DataAccessLayer.Filters;

namespace MikyM.Common.EfCore.DataAccessLayer.Pagination;

/// <inheritdoc />
[PublicAPI]
public class UriService : IUriService
{
    private readonly string _baseUri;

    /// <summary>
    /// Creates new instance of the Uri service.
    /// </summary>
    /// <param name="baseUri"></param>
    public UriService(string baseUri)
    {
        _baseUri = baseUri;
    }

    /// <inheritdoc />
    public Uri GetPageUri(PaginationFilter filter, string route, IQueryCollection? queryParams = null)
    {
        var endpointUri = string.Concat(_baseUri, route);

        if (queryParams is not null)
        {
            var query = queryParams.Where(x =>
                !string.Equals(x.Key.ToLower(), "pagesize", StringComparison.InvariantCultureIgnoreCase) &&
                !string.Equals(x.Key.ToLower(), "pagenumber", StringComparison.InvariantCultureIgnoreCase));

            endpointUri = query.Aggregate(endpointUri,
                (currentOuter, param) => param.Value.Aggregate(currentOuter,
                    (currentInner, multiParam) =>
                        QueryHelpers.AddQueryString(currentInner, param.Key, multiParam)));
        }

        endpointUri = QueryHelpers.AddQueryString(endpointUri, "pageNumber", filter.PageNumber.ToString());
        endpointUri = QueryHelpers.AddQueryString(endpointUri, "pageSize", filter.PageSize.ToString());

        return new Uri(endpointUri);
    }
}
