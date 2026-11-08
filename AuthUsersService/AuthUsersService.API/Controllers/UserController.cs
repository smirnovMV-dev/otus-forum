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
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class UserController : ControllerBase
{
    private const string RefreshTokenCookieName = "refresh_token";

    private readonly IMediator _mediator;
    private readonly RefreshTokenService _refreshTokenService;
    private readonly IUserRepository _userRepository;
    private readonly bool _cookieSecure;
    private readonly SameSiteMode _cookieSameSite;

    public UserController(
        IMediator mediator,
        RefreshTokenService refreshTokenService,
        IUserRepository userRepository,
        IConfiguration configuration)
    {
        _mediator = mediator;
        _refreshTokenService = refreshTokenService;
        _userRepository = userRepository;
        _cookieSecure = configuration.GetValue("Auth:Cookie:Secure", true);
        _cookieSameSite = Enum.TryParse<SameSiteMode>(
            configuration["Auth:Cookie:SameSite"],
            ignoreCase: true,
            out var sameSiteMode)
            ? sameSiteMode
            : SameSiteMode.Strict;
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

    [EnableRateLimiting("login")]
    [HttpPost("Login")]
    public async Task<LoginResponse> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(
            request.Email,
            request.Password);

        var result = await _mediator.Send(command, cancellationToken);

        AppendRefreshCookie(result.RefreshToken);

        return new LoginResponse
        {
            Token = result.Token,
            RefreshToken = result.RefreshToken,
            UserId = result.UserId,
            Nickname = result.Nickname,
            IsAdmin = result.IsAdmin,
            IsModerator = result.IsModerator
        };
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var refreshToken = ResolveRefreshToken(request);

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized(new { message = "No refresh token found." });
        }

        var result = await _refreshTokenService.RefreshAsync(refreshToken, cancellationToken);

        if (!result.IsSuccess || result.Token is null || result.RefreshToken is null)
        {
            return Unauthorized(new { message = result.Error });
        }

        AppendRefreshCookie(result.RefreshToken);

        return Ok(new RefreshResponse
        {
            Token = result.Token,
            RefreshToken = result.RefreshToken
        });
    }

    [HttpPost("Revoke")]
    public async Task<IActionResult> Revoke(
        [FromBody] RefreshRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var refreshToken = ResolveRefreshToken(request);

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Ok(new { message = "No token to revoke." });
        }

        await _refreshTokenService.RevokeAsync(refreshToken, cancellationToken);

        Response.Cookies.Delete(RefreshTokenCookieName);

        return Ok(new { message = "Logged out successfully." });
    }

    [HttpPost("Me")]
    public async Task<MeResponse> Me(
        [FromBody] RefreshRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var refreshToken = ResolveRefreshToken(request);

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return new MeResponse { IsAuthenticated = false };
        }

        var userId = await _refreshTokenService.GetSessionUserIdAsync(refreshToken, cancellationToken);

        if (userId is null)
        {
            return new MeResponse { IsAuthenticated = false };
        }

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);

        if (user is null)
        {
            return new MeResponse { IsAuthenticated = false };
        }

        var roles = await _userRepository.GetUserRoleNamesAsync(user.Id, cancellationToken);

        return new MeResponse
        {
            IsAuthenticated = true,
            UserId = user.Id,
            Nickname = user.Nickname,
            IsAdmin = roles.Contains(RoleName.Admin),
            IsModerator = roles.Contains(RoleName.Moderator)
        };
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

    private string? ResolveRefreshToken(RefreshRequest? request)
    {
        if (!string.IsNullOrWhiteSpace(request?.RefreshToken))
        {
            return request.RefreshToken;
        }

        var cookieValue = Request.Cookies[RefreshTokenCookieName];

        return string.IsNullOrWhiteSpace(cookieValue) ? null : cookieValue;
    }

    private void AppendRefreshCookie(string refreshToken)
    {
        Response.Cookies.Append(RefreshTokenCookieName, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = _cookieSecure,
            SameSite = _cookieSameSite,
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/"
        });
    }
}