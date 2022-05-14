namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Exceptions;

public class DuplicatePaginationException : Exception
{
    private new const string Message =
        "Duplicate use of the WithPaginatonFilter(). Ensure you don't use WithPaginationFilter() two times in the same specification!";

    public DuplicatePaginationException()
        : base(Message)
    {
    }

    public DuplicatePaginationException(Exception innerException)
        : base(Message, innerException)
    {
    }
}