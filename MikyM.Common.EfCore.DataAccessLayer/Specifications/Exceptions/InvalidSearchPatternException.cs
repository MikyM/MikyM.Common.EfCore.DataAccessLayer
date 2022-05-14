namespace MikyM.Common.EfCore.DataAccessLayer.Specifications.Exceptions;

public class InvalidSearchPatternException : Exception
{
    private const string message = "Invalid search pattern: ";

    public InvalidSearchPatternException(string searchPattern)
        : base($"{message}{searchPattern}")
    {
    }

    public InvalidSearchPatternException(string searchPattern, Exception innerException)
        : base($"{message}{searchPattern}", innerException)
    {
    }
}