using System.Collections.Generic;

namespace OtusForum.UI.Clients.Users;

public record UserSearchModel(long Id, string Nickname, string Email);

public record RoleModel(long Id, string Name);

public record GetAllRolesResponse(IReadOnlyCollection<RoleModel> Roles);

public record UserWithRolesModel(long Id, string Nickname, string Email, IReadOnlyCollection<string> CurrentRoles);
