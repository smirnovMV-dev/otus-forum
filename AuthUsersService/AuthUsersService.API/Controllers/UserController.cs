using AuthUsersService.API.Models.Auth;
using AuthUsersService.API.Models.Users.AssignUserRole;
using AuthUsersService.API.Models.Users.ChangePassword;
using AuthUsersService.API.Models.Users.RegisterUser;
using AuthUsersService.API.Models.Users.RemoveUserRole;
using AuthUsersService.Application.Auth.LoginUser;
using AuthUsersService.Application.Roles.GetAllRoles;
using AuthUsersService.Application.Users;
using AuthUsersService.Application.Users.AssignUserRole;
using AuthUsersService.Application.Users.ChangePassword;
using AuthUsersService.Application.Users.CountUsers;
using AuthUsersService.Application.Users.GetUserWithRoles;
using AuthUsersService.Application.Users.RegisterUser;
using AuthUsersService.Application.Users.RemoveUserRole;
using AuthUsersService.Application.Users.SearchUsersByNickname;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
        return new LoginResponse
        {
            Token = result.Token,
            UserId = result.UserId,
            Nickname = result.Nickname,
            IsAdmin = result.IsAdmin,
            IsModerator = result.IsModerator
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
    public async Task<IActionResult> SearchByNickname(
        [FromQuery] string nicknamePart,
        [FromHeader(Name = "X-IsAdmin")] bool isAdmin)
    {
        if (!isAdmin)
            return StatusCode(403);

        var command = new SearchUsersByNicknameQuery(nicknamePart);
        return Ok(await _mediator.Send(command));
    }

    [HttpGet("GetWithRoles/{userId}")]
    public async Task<IActionResult> GetWithRoles(long userId,
        [FromHeader(Name = "X-IsAdmin")] bool isAdmin)
    {
        if (!isAdmin)
            return StatusCode(403);

        var command = new GetUserWithRolesQuery(userId);
        return Ok(await _mediator.Send(command));
    }

    [HttpDelete("RemoveRole")]
    public async Task<IActionResult> RemoveRole(
        [FromBody] RemoveUserRoleRequest request,
        [FromHeader(Name = "X-IsAdmin")] bool isAdmin)
    {
        if (!isAdmin)
            return StatusCode(403);

        var command = new RemoveUserRoleCommand(request.UserId, request.RoleId);
        return Ok(await _mediator.Send(command));
    }
}
