using System;
using BBB_ApplicationDashboard.Infrastructure.Exceptions.Common;

namespace BBB_ApplicationDashboard.Infrastructure.Exceptions.User;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(string message)
        : base(message) { }

    public UserNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}
