using System.Collections.Generic;

namespace AuthUsersService.Application.Users;

public record UserWithRolesModel(
    long Id,
    string Nickname,
    string Email,
    ICollection<long> CurrentRoles);
