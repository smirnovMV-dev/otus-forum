using AuthUsersService.Application.Auth.Models;
using AuthUsersService.Application.Auth.LoginUser.Models;
using AuthUsersService.Application.Auth.Tokens;
using AuthUsersService.Domain;
using AuthUsersService.Application.Users;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RT = AuthUsersService.Domain.Entities.RefreshToken;

namespace AuthUsersService.Application.Auth.LoginUser;

public sealed record LoginUserCommand(
    string Nickname,
    string Password) : IRequest<LoginUserResponse>;

internal sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<LoginUserResponse> Handle(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByNicknameAsync(command.Nickname, cancellationToken);

        if (user == null || !BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Неверный никнейм или пароль.");
        }

        var roles = await _userRepository.GetUserRoleNamesAsync(user.Id, cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user, roles);

        // Generate refresh token
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        var refreshTokenHash = RefreshTokenHasher.ComputeHash(refreshToken);

        // Revoke all existing refresh tokens for this user
        await _refreshTokenRepository.RevokeAllForUserAsync(
            user.Id,
            RefreshTokenRevokeReason.RevokedOnLogin,
            cancellationToken);

        // Save new refresh token
        var newRefreshToken = new RT
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

        var isAdmin = roles.Contains(RoleName.Admin);
        var isModerator = roles.Contains(RoleName.Moderator);

        return new LoginUserResponse(token, refreshToken, user.Id, user.Nickname, isAdmin, isModerator);
    }
}
