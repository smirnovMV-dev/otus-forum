using AuthUsersService.Domain;
using AuthUsersService.Domain.Entities;
using System.Collections.Generic;

namespace AuthUsersService.Application.Auth.Models;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, IReadOnlyCollection<AccessLevel> accessLevels);
}
