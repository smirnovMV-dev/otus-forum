using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace AuthUsersService.Application.Users.SearchUsersByNickname;

public sealed record SearchUsersByNicknameQuery(string NicknamePart) : IRequest<IReadOnlyCollection<UserSearchModel>>;

internal sealed class SearchUsersByNicknameQueryHandler : IRequestHandler<SearchUsersByNicknameQuery, IReadOnlyCollection<UserSearchModel>>
{
    private readonly IUserRepository _userRepository;

    public SearchUsersByNicknameQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyCollection<UserSearchModel>> Handle(
        SearchUsersByNicknameQuery request,
        CancellationToken cancellationToken)
    {
        return await _userRepository.SearchByNicknameAsync(request.NicknamePart, cancellationToken);
    }
}
