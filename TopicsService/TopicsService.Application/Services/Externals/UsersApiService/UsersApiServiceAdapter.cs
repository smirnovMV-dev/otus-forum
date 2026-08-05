using Microsoft.Extensions.Logging;
using OtusForum.AuthUsersService.Grpc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Application.Services.Externals.UsersApiService.Models;

namespace TopicsService.Application.Services.Externals.UsersApiService;

internal sealed class UsersApiServiceAdapter : IUsersApiServiceAdapter
{
    private readonly UsersGrpcApi.UsersGrpcApiClient _client;
    private readonly ILogger<UsersApiServiceAdapter> _logger;

    public UsersApiServiceAdapter(
        UsersGrpcApi.UsersGrpcApiClient client,
        ILogger<UsersApiServiceAdapter> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<IReadOnlyDictionary<long, UserNickname>> GetUsersNicknamesAsync(
        IReadOnlyCollection<long> userIds,
        CancellationToken cancellationToken)
    {
        if (userIds.Count == 0)
        {
            return ReadOnlyDictionary<long, UserNickname>.Empty;
        }

        try
        {
            var request = new GetNicknamesRequest
            {
                UserIds = { userIds },
            };

            var response = await _client.GetNicknamesAsync(
                request,
                cancellationToken: cancellationToken);

            return response
                .UsersNicknames
                .ToDictionary(
                x => x.UserId,
                x => new UserNickname(x.UserId, x.Nickname));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.Message);
            return ReadOnlyDictionary<long, UserNickname>.Empty; ;
        }
    }
}
