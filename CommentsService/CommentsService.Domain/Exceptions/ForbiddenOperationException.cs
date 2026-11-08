using System;

namespace CommentsService.Domain.Exceptions;

/// <summary>
/// Операция запрещена для текущего пользователя.
/// </summary>
public sealed class ForbiddenOperationException : Exception
{
    public ForbiddenOperationException()
        : base("Недостаточно прав")
    {
    }

    public ForbiddenOperationException(string message)
        : base(message)
    {
    }
}
