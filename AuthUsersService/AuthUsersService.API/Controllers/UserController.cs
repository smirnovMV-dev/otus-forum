using AuthUsersService.API.Models.Auth;
using AuthUsersService.API.Models.Users.AssignUserRole;
using AuthUsersService.API.Models.Users.RegisterUser;
using AuthUsersService.Application.Auth.LoginUser;
using AuthUsersService.Application.Users.AssignUserRole;
using AuthUsersService.Application.Users.CountUsers;
using AuthUsersService.Application.Users.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
            Nickname = result.Nickname
        };
    }

    [HttpGet("Count")]
    public async Task<int> Count()
    {
        var command = new CountUsersCommand();
        return await _mediator.Send(command);
    }
}
