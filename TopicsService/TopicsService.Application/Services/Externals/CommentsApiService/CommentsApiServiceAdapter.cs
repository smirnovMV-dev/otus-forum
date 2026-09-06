using Microsoft.Extensions.Logging;
using OtusForum.CommentsService.Grpc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Application.Services.Externals.CommentsApiService.Models;

namespace TopicsService.Application.Services.Externals.CommentsApiService;

internal sealed class CommentsApiServiceAdapter : ICommentsApiServiceAdapter
{
    private readonly CommentsGrpcApi.CommentsGrpcApiClient _client;
    private readonly ILogger<CommentsApiServiceAdapter> _logger;

    public CommentsApiServiceAdapter(
        CommentsGrpcApi.CommentsGrpcApiClient client,
        ILogger<CommentsApiServiceAdapter> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<CommentDto>> GetCommentsByTopicIdAsync(
        long topicId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetCommentsByTopicRequest
            {
                TopicId = topicId
            };

            var response = await _client.GetCommentsByTopicAsync(
                request,
                cancellationToken: cancellationToken);

            return response.Comments
                .Select(c => new CommentDto(
                    c.Id,
                    c.TopicId,
                    c.ParentCommentId == 0 ? (long?)null : c.ParentCommentId,
                    c.AuthorId,
                    c.Content,
                    DateTimeOffset.Parse(c.CreatedAt)))
                .ToList()
                .AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении комментариев для темы {TopicId}", topicId);
            return new List<CommentDto>().AsReadOnly();
        }
    }
}
