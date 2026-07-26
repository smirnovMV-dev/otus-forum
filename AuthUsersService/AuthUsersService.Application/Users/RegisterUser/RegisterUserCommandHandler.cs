using AuthUsersService.Domain.Entities;
using AuthUsersService.Infrastructure.Repositories.Users;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Nickname,
    string Email,
    string Password) : IRequest<int>;

internal class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, int>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<int> Handle(
        RegisterUserCommand command, 
        CancellationToken cancellationToken)
    {
        var user = User.Create(
            command.Nickname,
            command.Email,
            command.Password,
            DateTimeOffset.UtcNow);

        return await _userRepository.CreateAsync(
            user,
            cancellationToken);
    }
}
