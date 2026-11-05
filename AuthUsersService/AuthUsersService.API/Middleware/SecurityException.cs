using System;

namespace AuthUsersService.API.Middleware;

public sealed class SecurityException : Exception
{
    public SecurityException()
        : base("Недостаточно прав")
    {
    }

    public SecurityException(string message)
        : base(message)
    {
    }
}
