namespace MikyM.Common.EfCore.DataAccessLayer.Filters;

/// <summary>
/// Pagination filter
/// </summary>
public class PaginationFilter
{
    public PaginationFilter()
    {
        PageNumber = 1;
        PageSize = 10;
    }

    public PaginationFilter(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize > 10 ? 10 : pageSize;
    }

    /// <summary>
    /// Page number
    /// </summary>
    public int PageNumber { get; set; }
    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; }
}