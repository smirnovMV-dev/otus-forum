using CommentsService.API.Models.Comments.CreateComment;
using CommentsService.Application.Comments.CreateComment;
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
}
