using System.Collections.Generic;

using AuthUsersService.Domain;

namespace AuthUsersService.Application.Users;

public record UserWithRolesModel(
    long Id,
    string Nickname,
    string Email,
    IReadOnlyCollection<RoleName> CurrentRoles);
