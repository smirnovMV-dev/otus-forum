using System;
using System.Security.Cryptography;
using System.Text;

namespace AuthUsersService.Application.Auth.Tokens;

public static class RefreshTokenHasher
{
    public static string ComputeHash(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
