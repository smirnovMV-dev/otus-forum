using AuthUsersService.API.Middleware;
using AuthUsersService.API.Models.Roles.CreateRole;
using AuthUsersService.API.Models.Roles.GetAllRoles;
using AuthUsersService.Application.Roles.CountRoles;
using AuthUsersService.Application.Roles.CreateRole;
using AuthUsersService.Application.Roles.GetAllRoles;
using AuthUsersService.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
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
        var roleName = RoleNameExtensions.FromString(request.RoleName);
        var command = new CreateRoleCommand(roleName);

        await _mediator.Send(command);
        return new CreateRoleResponse();
    }

    [HttpGet("CountRoles")]
    public async Task<int> Count()
    {
        var command = new CountRolesCommand();
        return await _mediator.Send(command);
    }

    [HttpGet("GetAll")]
    public async Task<GetAllRolesResponse> GetAll(
        [FromHeader(Name = "X-IsAdmin")] bool isAdmin)
    {
        if (!isAdmin)
            throw new SecurityException();

        var command = new GetAllRolesCommand();
        var roles = await _mediator.Send(command);

        var models = roles
            .Select(r => new RoleModel
            {
                Id = r.Id,
                Name = r.Name
            })
            .ToList();

        return new GetAllRolesResponse { Roles = models };
    }
}
