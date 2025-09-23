namespace BBB_ApplicationDashboard.Infrastructure.Exceptions.Common;

public class InternalServerException : Exception
{
    public InternalServerException(string message)
        : base(message) { }

    public InternalServerException(string message, Exception innerException)
        : base(message, innerException) { }
}
