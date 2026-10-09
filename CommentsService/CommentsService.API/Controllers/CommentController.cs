using CommentsService.API.Models.Comments.CreateComment;
using CommentsService.API.Models.Comments.DeleteComment;
using CommentsService.API.Models.Comments.UpdateComment;
using CommentsService.Application.Comments.CreateComment;
using CommentsService.Application.Comments.DeleteComment;
using CommentsService.Application.Comments.UpdateComment;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CommentsService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost(nameof(Create))]
    public async Task<CreateCommentResponse> Create(
        [FromBody] CreateCommentRequest request,
        [FromHeader(Name = "X-UserId")] long userId)
    {
        var command = new CreateCommentCommand(
            request.TopicId,
            request.ParentCommentId,
            userId,
            request.Content);

        await _mediator.Send(command);

        return new CreateCommentResponse();
    }

    [HttpPut(nameof(Update))]
    public async Task<UpdateCommentResponse> Update(
        long id,
        [FromBody] UpdateCommentRequest request,
        [FromHeader(Name = "X-UserId")] long updatedByUserId)
    {
        var command = new UpdateCommentCommand(
            id,
            request.TopicId,
            request.ParentCommentId,
            updatedByUserId,
            request.Content);

        await _mediator.Send(command);

        return new UpdateCommentResponse();
    }

    [HttpDelete(nameof(Delete))]
    public async Task<DeleteCommentResponse> Delete(
        long id)
    {
        var command = new DeleteCommentCommand(id);

        await _mediator.Send(command);

        return new DeleteCommentResponse();
    }
}
