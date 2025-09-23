using System;
using BBB_ApplicationDashboard.Infrastructure.Exceptions.Common;

namespace BBB_ApplicationDashboard.Infrastructure.Exceptions.User;

public class UserBadRequestException : BadRequestException
{
    public UserBadRequestException(string message)
        : base(message) { }

    public UserBadRequestException(string message, Exception innerException)
        : base(message, innerException) { }
}
