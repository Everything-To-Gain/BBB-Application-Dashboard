using System;
using BBB_ApplicationDashboard.Infrastructure.Exceptions.Common;

namespace BBB_ApplicationDashboard.Infrastructure.Exceptions.User;

public class UserUnauthorizedException : UnauthorizedException
{
    public UserUnauthorizedException(string message)
        : base(message) { }

    public UserUnauthorizedException(string message, Exception innerException)
        : base(message, innerException) { }
}
