using AuthUsersService.Domain;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Application.Users.ChangePassword;

internal sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, int>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<int> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user == null || !BCrypt.Net.BCrypt.Verify(command.CurrentPassword, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Неверный текущий пароль.");
        }

        var newHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword);
        await _userRepository.UpdatePasswordAsync(command.UserId, newHash, cancellationToken);

        // После смены пароля все выданные ранее refresh-токены недействительны.
        await _refreshTokenRepository.RevokeAllForUserAsync(
            command.UserId,
            RefreshTokenRevokeReason.PasswordChanged,
            cancellationToken);

        return 1;
    }
}
