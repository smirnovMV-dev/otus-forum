using System.Collections.Generic;

namespace AuthUsersService.Domain;

public static class RoleNameMapping
{
    private static readonly Dictionary<string, RoleName> NameToRole = new()
    {
        { "Admin", RoleName.Admin },
        { "Moderator", RoleName.Moderator },
        { "User", RoleName.User }
    };

    public static RoleName FromName(string name) =>
        NameToRole.TryGetValue(name, out var role) ? role : RoleName.Unknown;
}
