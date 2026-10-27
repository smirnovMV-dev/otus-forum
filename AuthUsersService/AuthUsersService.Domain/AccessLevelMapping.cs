using System.Collections.Generic;

namespace AuthUsersService.Domain;

public static class AccessLevelMapping
{
    private static readonly Dictionary<string, AccessLevel> NameToLevel = new()
    {
        { "Admin", AccessLevel.Admin },
        { "Moderator", AccessLevel.Moderator },
        { "User", AccessLevel.User }
    };

    public static AccessLevel FromName(string name) =>
        NameToLevel.TryGetValue(name, out var level) ? level : AccessLevel.Unknown;
}
