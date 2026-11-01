using System.Collections.Generic;

namespace AuthUsersService.API.Models.Roles.GetAllRoles;

public sealed class GetAllRolesResponse
{
    public IReadOnlyCollection<RoleModel> Roles { get; init; } = new List<RoleModel>();
}

public sealed class RoleModel
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
