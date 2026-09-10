using AuthUsersService.API.Models.Roles.CreateRole;
using AuthUsersService.Application.Roles.CountRoles;
using AuthUsersService.Application.Roles.CreateRole;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthUsersService.API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class RoleController : ControllerBase
{
    private readonly IMediator _mediator;

    public RoleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost(nameof(Create))]
    public async Task<CreateRoleResponse> Create(
        [FromBody] CreateRoleRequest request)
    {
        var command = new CreateRoleCommand(
            request.RoleName);

        await _mediator.Send(command);
        return new CreateRoleResponse();
    }

    [HttpGet("CountRoles")]
    public async Task<int> Count()
    {
        var command = new CountRolesCommand();
        return await _mediator.Send(command);
    }
}
