using CommentsService.Application.Comments.GetCommentsByTopic;
using CommentsService.Application.Comments.GetCommentsByTopic.Models;
using Grpc.Core;
using MediatR;
using CommentProto = OtusForum.CommentsService.Grpc.Comment;
using GetCommentsByTopicRequestProto = OtusForum.CommentsService.Grpc.GetCommentsByTopicRequest;
using GetCommentsByTopicResponseProto = OtusForum.CommentsService.Grpc.GetCommentsByTopicResponse;

namespace CommentsService.API.Grpc;

public sealed class CommentsGrpcService : OtusForum.CommentsService.Grpc.CommentsGrpcApi.CommentsGrpcApiBase
{
    private readonly IMediator _mediator;

    public CommentsGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async System.Threading.Tasks.Task<GetCommentsByTopicResponseProto> GetCommentsByTopic(
        GetCommentsByTopicRequestProto request,
        ServerCallContext context)
    {
        var page = request.Page > 0 ? request.Page : 1;
        var pageSize = request.PageSize > 0 ? request.PageSize : 20;

        var result = await _mediator.Send(
            new GetCommentsByTopicCommand(request.TopicId, page, pageSize),
            context.CancellationToken);

        var response = new GetCommentsByTopicResponseProto
        {
            TotalCount = result.TotalCount
        };

        foreach (var comment in result.Comments)
        {
            response.Comments.Add(MapToCommentProto(comment));
        }

        return response;
    }

    private CommentProto MapToCommentProto(CommentTreeDto dto)
    {
        var proto = new CommentProto
        {
            Id = dto.Id,
            TopicId = dto.TopicId,
            AuthorId = dto.AuthorId,
            Content = dto.Content,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };

        if (dto.ParentCommentId.HasValue)
        {
            proto.ParentCommentId = dto.ParentCommentId.Value;
        }

        foreach (var reply in dto.Replies)
        {
            proto.Replies.Add(MapToCommentProto(reply));
        }

        return proto;
    }
}
