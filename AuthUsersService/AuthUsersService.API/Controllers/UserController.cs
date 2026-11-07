using AuthUsersService.API.Middleware;
using AuthUsersService.API.Models.Auth;
using AuthUsersService.API.Models.Users.AssignUserRole;
using AuthUsersService.API.Models.Users.ChangePassword;
using AuthUsersService.API.Models.Users.RegisterUser;
using AuthUsersService.API.Models.Users.RemoveUserRole;
using AuthUsersService.Application.Auth.LoginUser;
using AuthUsersService.Application.Auth.RefreshToken;
using AuthUsersService.Application.Roles.GetAllRoles;
using AuthUsersService.Application.Users;
using AuthUsersService.Application.Users.AssignUserRole;
using AuthUsersService.Application.Users.ChangePassword;
using AuthUsersService.Application.Users.CountUsers;
using AuthUsersService.Application.Users.GetUserWithRoles;
using AuthUsersService.Application.Users.RegisterUser;
using AuthUsersService.Application.Users.RemoveUserRole;
using AuthUsersService.Application.Users.SearchUsersByNickname;
using AuthUsersService.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost(nameof(Register))]
    public async Task<RegisterUserResponse> Register(
        [FromBody] RegisterUserRequest request)
    {
        var command = new RegisterUserCommand(
            request.Nickname,
            request.Email,
            request.Password);
        
        await _mediator.Send(command);
        return new RegisterUserResponse();
    }

    [HttpPost(nameof(AssignRole))]
    public async Task<AssignUserRoleResponse> AssignRole(
        [FromBody] AssignUserRoleRequest request)
    {
        var command = new AssignUserRoleCommand(
            request.UserId,
            request.RoleId,
            request.ExpiresAt);

        await _mediator.Send(command);
        return new AssignUserRoleResponse();
    }

    [HttpPost("Login")]
    public async Task<LoginResponse> Login(
        [FromBody] LoginRequest request)
    {
        var command = new LoginUserCommand(
            request.Email,
            request.Password);

        var result = await _mediator.Send(command);

        // Set httpOnly cookie with refresh token
        Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/"
        });

        return new LoginResponse
        {
            Token = result.Token,
            UserId = result.UserId,
            Nickname = result.Nickname,
            IsAdmin = result.IsAdmin,
            IsModerator = result.IsModerator
        };
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshTokenCookie = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshTokenCookie))
        {
            return Unauthorized(new { message = "No refresh token found." });
        }

        // Get userId from JWT header (set by GatewayService middleware)
        if (!long.TryParse(Request.Headers["X-UserId"], out var userId))
        {
            return Unauthorized(new { message = "Invalid user identity." });
        }

        var refreshTokenService = HttpContext.RequestServices.GetRequiredService<RefreshTokenService>();
        var result = await refreshTokenService.RefreshAsync(refreshTokenCookie, userId, CancellationToken.None);

        if (!result.IsSuccess)
        {
            return Unauthorized(new { message = result.Error });
        }

        return Ok(new { token = result.Token });
    }

    [HttpPost("Revoke")]
    public async Task<IActionResult> Revoke()
    {
        var refreshTokenCookie = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshTokenCookie))
        {
            return Ok(new { message = "No token to revoke." });
        }

        var refreshTokenService = HttpContext.RequestServices.GetRequiredService<RefreshTokenService>();
        await refreshTokenService.RevokeAsync(refreshTokenCookie, CancellationToken.None);

        Response.Cookies.Delete("refresh_token");

        return Ok(new { message = "Logged out successfully." });
    }

    [HttpGet("Me")]
    public async Task<IActionResult> Me()
    {
        var refreshTokenCookie = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshTokenCookie))
        {
            return Ok(new { isAuthenticated = false });
        }

        var refreshTokenService = HttpContext.RequestServices.GetRequiredService<RefreshTokenService>();
        var isValid = await refreshTokenService.IsSessionValidAsync(refreshTokenCookie, CancellationToken.None);

        return Ok(new { isAuthenticated = isValid });
    }

    [HttpGet("CountUsers")]
    public async Task<int> Count()
    {
        var command = new CountUsersCommand();
        return await _mediator.Send(command);
    }

    [HttpPost("ChangePassword")]
    public async Task<ChangePasswordResponse> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        [FromHeader(Name = "X-UserId")] long userId)
    {
        var command = new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword);
        await _mediator.Send(command);
        return new ChangePasswordResponse();
    }

    [HttpGet("SearchByNickname")]
    public async Task<IReadOnlyCollection<UserSearchModel>> SearchByNickname(
        [FromQuery] string nicknamePart,
        [FromHeader(Name = "X-IsAdmin")] bool isAdmin)
    {
        if (!isAdmin)
            throw new SecurityException();

        var command = new SearchUsersByNicknameQuery(nicknamePart);
        return await _mediator.Send(command);
    }

    [HttpGet("GetWithRoles/{userId}")]
    public async Task<UserWithRolesModel> GetWithRoles(long userId,
        [FromHeader(Name = "X-IsAdmin")] bool isAdmin)
    {
        if (!isAdmin)
            throw new SecurityException();

        var command = new GetUserWithRolesQuery(userId);
        return await _mediator.Send(command);
    }

    [HttpDelete("RemoveRole")]
    public async Task RemoveRole(
        [FromBody] RemoveUserRoleRequest request,
        [FromHeader(Name = "X-IsAdmin")] bool isAdmin)
    {
        if (!isAdmin)
            throw new SecurityException();

        var command = new RemoveUserRoleCommand(request.UserId, request.RoleId);
        await _mediator.Send(command);
    }
}
