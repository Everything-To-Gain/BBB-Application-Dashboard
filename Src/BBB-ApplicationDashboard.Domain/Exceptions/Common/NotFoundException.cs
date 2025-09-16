namespace BBB_ApplicationDashboard.Domain.Exceptions.Common;

public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message) { }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}
