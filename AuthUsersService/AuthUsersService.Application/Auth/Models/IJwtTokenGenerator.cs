using AuthUsersService.Domain;
using AuthUsersService.Domain.Entities;
using System.Collections.Generic;

namespace AuthUsersService.Application.Auth.Models;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, IReadOnlyCollection<RoleName> roleNames);
    string GenerateTokenFromUserId(long userId, IReadOnlyCollection<RoleName> roleNames);
    string GenerateRefreshToken();
}
