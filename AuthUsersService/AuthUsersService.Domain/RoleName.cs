namespace AuthUsersService.Domain;

public enum RoleName
{
    Unknown = 0,
    User = 1,
    Admin = 2,
    Moderator = 3
}

public static class RoleNameExtensions
{
    public static RoleName FromString(string name) => name switch
    {
        "Admin" => RoleName.Admin,
        "Moderator" => RoleName.Moderator,
        "User" => RoleName.User,
        _ => RoleName.Unknown
    };
}
