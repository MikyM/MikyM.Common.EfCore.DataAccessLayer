namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Exceptions;

public class DuplicateSkipException : Exception
{
    private new const string Message =
        "Duplicate use of the Skip(). Ensure you don't use both WithPaginationFilter() and Skip() in the same specification!";

    public DuplicateSkipException()
        : base(Message)
    {
    }

    public DuplicateSkipException(Exception innerException)
        : base(Message, innerException)
    {
    }
}