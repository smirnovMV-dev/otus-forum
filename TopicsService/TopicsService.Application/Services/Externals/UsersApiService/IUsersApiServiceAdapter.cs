using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Application.Services.Externals.UsersApiService.Models;

namespace TopicsService.Application.Services.Externals.UsersApiService;

internal interface IUsersApiServiceAdapter
{
    Task<IReadOnlyDictionary<long, UserNickname>> GetUsersNicknamesAsync(
        IReadOnlyCollection<long> userIds,
        CancellationToken cancellationToken);
}
