using System.ComponentModel.DataAnnotations;

namespace AuthUsersService.API.Models.Users.RemoveUserRole;

public sealed class RemoveUserRoleRequest
{
    [Required]
    [RegularExpression(@"^-?(?:0|[1-9]\d*)$")]
    public long UserId { get; set; }

    [Required]
    [RegularExpression(@"^-?(?:0|[1-9]\d*)$")]
    public long RoleId { get; set; }
}
