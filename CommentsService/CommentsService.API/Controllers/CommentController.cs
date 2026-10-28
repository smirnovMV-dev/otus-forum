using CommentsService.API.Models.Comments.CreateComment;
using CommentsService.API.Models.Comments.DeleteComment;
using CommentsService.API.Models.Comments.GetCommentsByUserId;
using CommentsService.API.Models.Comments.UpdateComment;
using CommentsService.Application.Comments.CreateComment;
using CommentsService.Application.Comments.DeleteComment;
using CommentsService.Application.Comments.GetCommentsByUserId;
using CommentsService.Application.Comments.UpdateComment;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
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
        [FromHeader(Name = "X-UserId")] long userId,
        [FromHeader(Name = "X-IsAdmin")] bool isAdmin = false,
        [FromHeader(Name = "X-IsModerator")] bool isModerator = false)
    {
        var command = new CreateCommentCommand(
            request.TopicId,
            request.ParentCommentId,
            userId,
            request.Content,
            isAdmin,
            isModerator);

        await _mediator.Send(command);

        return new CreateCommentResponse();
    }

    [HttpPut(nameof(Update))]
    public async Task<UpdateCommentResponse> Update(
        long id,
        [FromBody] UpdateCommentRequest request,
        [FromHeader(Name = "X-UserId")] long updatedByUserId,
        [FromHeader(Name = "X-IsAdmin")] bool isAdmin = false,
        [FromHeader(Name = "X-IsModerator")] bool isModerator = false)
    {
        var command = new UpdateCommentCommand(
            id,
            request.TopicId,
            request.ParentCommentId,
            updatedByUserId,
            request.Content,
            isAdmin,
            isModerator);

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

    [HttpPost("GetByUserId")]
    public async Task<GetCommentsByUserIdResponse> GetByUserId(
        [FromBody] GetCommentsByUserIdRequest request)
    {
        var command = new GetCommentsByUserIdCommand(request.UserId, request.Page, request.PageSize);
        var results = await _mediator.Send(command);

        return new GetCommentsByUserIdResponse
        {
            Comments = [.. results.Comments.Select(r => new GetCommentsByUserIdItemResponse
            {
                Id = r.Id,
                TopicId = r.TopicId,
                ParentCommentId = r.ParentCommentId,
                AuthorNickname = r.AuthorNickname,
                Content = r.Content,
                CreatedAt = r.CreatedAt
            })],
            TotalCount = results.TotalCount
        };
    }
}
