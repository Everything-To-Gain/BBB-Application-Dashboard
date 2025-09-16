using BBB_ApplicationDashboard.Domain.Exceptions.Common;

namespace BBB_ApplicationDashboard.Domain.Exceptions.Email;

public class EmailBadRequestException : BadRequestException
{
    public EmailBadRequestException(string message)
        : base(message) { }

    public EmailBadRequestException(string message, Exception innerException)
        : base(message, innerException) { }
}
