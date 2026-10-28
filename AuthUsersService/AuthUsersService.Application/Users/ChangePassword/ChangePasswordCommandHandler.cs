using AuthUsersService.Application.Users.ChangePassword;
using AuthUsersService.Domain.Entities;
using AuthUsersService.Infrastructure.Repositories.Users;
using MediatR;
using BCrypt.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Application.Users.ChangePassword;

internal sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, int>
{
    private readonly IUserRepository _userRepository;

    public ChangePasswordCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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
        return 1;
    }
}
