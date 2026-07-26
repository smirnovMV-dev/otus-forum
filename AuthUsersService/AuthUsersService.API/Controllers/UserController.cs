using AuthUsersService.API.Models.Users.AssignUserRole;
using AuthUsersService.API.Models.Users.RegisterUser;
using AuthUsersService.Application.Users.AssignUserRole;
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
}
