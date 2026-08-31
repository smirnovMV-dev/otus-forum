using AuthUsersService.Domain.Entities;

namespace AuthUsersService.Application.Auth.Models;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
