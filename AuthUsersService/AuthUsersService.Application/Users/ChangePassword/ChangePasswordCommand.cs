using AuthUsersService.Domain.Entities;
using MediatR;

namespace AuthUsersService.Application.Users.ChangePassword;

public sealed record ChangePasswordCommand(
    long UserId,
    string CurrentPassword,
    string NewPassword) : IRequest<int>;
