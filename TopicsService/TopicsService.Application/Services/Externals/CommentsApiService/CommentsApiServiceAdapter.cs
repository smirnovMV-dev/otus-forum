using Google.Protobuf.Collections;
using Microsoft.Extensions.Logging;
using OtusForum.CommentsService.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TopicsService.Application.Services.Externals.CommentsApiService.Models;
using CommentProto = OtusForum.CommentsService.Grpc.Comment;

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
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetCommentsByTopicRequest
            {
                TopicId = topicId,
                Page = page,
                PageSize = pageSize
            };

            var response = await _client.GetCommentsByTopicAsync(
                request,
                cancellationToken: cancellationToken);

            return response.Comments
                .Select(MapToCommentDto)
                .ToList()
                .AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении комментариев для темы {TopicId}", topicId);
            return new List<CommentDto>().AsReadOnly();
        }
    }

    private CommentDto MapToCommentDto(Comment proto)
    {
        var replies = proto.Replies
            .Select(MapToCommentDto)
            .ToList();

        return new CommentDto(
            proto.Id,
            proto.TopicId,
            proto.ParentCommentId == 0 ? (long?)null : proto.ParentCommentId,
            proto.AuthorId,
            proto.Content,
            proto.CreatedAt,
            proto.UpdatedAt,
            replies as IReadOnlyCollection<CommentDto>);
    }
}
