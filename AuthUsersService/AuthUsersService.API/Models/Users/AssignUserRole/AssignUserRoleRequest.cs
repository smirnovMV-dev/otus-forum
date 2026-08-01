using System;

namespace AuthUsersService.API.Models.Users.AssignUserRole;

public sealed class AssignUserRoleRequest
{
    public required long UserId { get; set; }

    public required long RoleId { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }
}
