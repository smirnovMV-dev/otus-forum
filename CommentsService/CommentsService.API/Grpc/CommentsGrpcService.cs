using CommentsService.Domain.Entities;
using CommentsService.Infrastructure.Repositories.Comments;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using CommentProto = OtusForum.CommentsService.Grpc.Comment;
using GetCommentsByTopicRequestProto = OtusForum.CommentsService.Grpc.GetCommentsByTopicRequest;
using GetCommentsByTopicResponseProto = OtusForum.CommentsService.Grpc.GetCommentsByTopicResponse;

namespace CommentsService.API.Grpc;

public sealed class CommentsGrpcService : OtusForum.CommentsService.Grpc.CommentsGrpcApi.CommentsGrpcApiBase
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<CommentsGrpcService> _logger;

    public CommentsGrpcService(
        ICommentRepository commentRepository,
        ILogger<CommentsGrpcService> logger)
    {
        _commentRepository = commentRepository;
        _logger = logger;
    }

    public override async System.Threading.Tasks.Task<GetCommentsByTopicResponseProto> GetCommentsByTopic(
        GetCommentsByTopicRequestProto request,
        ServerCallContext context)
    {
        var response = new GetCommentsByTopicResponseProto();
        var comments = await _commentRepository.GetByTopicIdAsync(
            request.TopicId,
            context.CancellationToken);

        foreach (var comment in comments)
        {
            response.Comments.Add(new CommentProto
            {
                Id = comment.Id,
                TopicId = comment.TopicId,
                ParentCommentId = comment.ParentCommentId ?? 0,
                AuthorId = comment.AuthorId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt.ToString("o"),
                UpdatedAt = comment.UpdatedAt.ToString("o")
            });
        }

        return response;
    }
}
