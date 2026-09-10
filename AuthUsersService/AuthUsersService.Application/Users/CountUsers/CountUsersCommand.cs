using AuthUsersService.Infrastructure.Repositories.Users;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Application.Users.CountUsers;

public sealed record CountUsersCommand() : IRequest<int>;

internal sealed class CountUsersCommandHandler : IRequestHandler<CountUsersCommand, int>
{
    private readonly IUserRepository _userRepository;

    public CountUsersCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<int> Handle(
        CountUsersCommand request,
        CancellationToken cancellationToken)
    {
        return await _userRepository.CountAsync(cancellationToken);
    }
}
