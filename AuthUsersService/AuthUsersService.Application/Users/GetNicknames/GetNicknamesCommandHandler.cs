using AuthUsersService.Application.Users.GetNicknames.Models;
using AuthUsersService.Infrastructure.Repositories.Users;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUsersService.Application.Users.GetNicknames;

public sealed record GetNicknamesCommand(
    IReadOnlyCollection<long> UserIds) : IRequest<IReadOnlyCollection<GetNicknamesResult>>;
internal sealed class GetNicknamesCommandHandler
    : IRequestHandler<GetNicknamesCommand, IReadOnlyCollection<GetNicknamesResult>>
{
    private readonly IUserRepository _userRepository;

    public GetNicknamesCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyCollection<GetNicknamesResult>> Handle(
        GetNicknamesCommand command,
        CancellationToken cancellationToken)
    {
        var results = await _userRepository.GetNicknamesAsync(
            command.UserIds,
            cancellationToken);

        return [.. results.Select(r => new GetNicknamesResult(
            r.UserId,
            r.Nickname))];
    }
}
