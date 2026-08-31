using AuthUsersService.Application.Auth.Models;
using AuthUsersService.Application.Auth.LoginUser.Models;
using AuthUsersService.Domain.Entities;
using AuthUsersService.Infrastructure.Repositories.Users;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Application.Auth.LoginUser;

public sealed record LoginUserCommand(
    string Email,
    string Password) : IRequest<LoginUserResponse>;

internal sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginUserResponse> Handle(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);

        if (user == null || !BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Неверный email или пароль.");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new LoginUserResponse(token, user.Id, user.Nickname);
    }
}
