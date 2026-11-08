using CommentsService.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CommentsService.API.Middleware;

/// <summary>
/// Преобразует доменные исключения безопасности в корректные HTTP-статусы.
/// </summary>
public sealed class SecurityExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ForbiddenOperationException exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await context.Response.WriteAsync(exception.Message);
        }
        catch (UnauthorizedAccessException exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync(exception.Message);
        }
    }
}
