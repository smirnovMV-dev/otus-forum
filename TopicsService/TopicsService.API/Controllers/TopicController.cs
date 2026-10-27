using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TopicsService.API.Models.Topics.CreateTopic;
using TopicsService.API.Models.Topics.GetLatestTopics;
using TopicsService.API.Models.Topics.GetTopicWithComments;
using TopicsService.API.Models.Topics.UpdateTopic;
using TopicsService.Application.Topics.CreateTopic;
using TopicsService.Application.Topics.DeleteTopic;
using TopicsService.Application.Topics.GetLatestTopics;
using TopicsService.Application.Topics.GetTopicWithComments;
using TopicsService.Application.Topics.UpdateTopic;

namespace TopicsService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TopicController : ControllerBase
{
    private readonly IMediator _mediator;

    public TopicController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost(nameof(Create))]
    public async Task<CreateTopicResponse> Create(
        [FromBody] CreateTopicRequest request,
        [FromHeader(Name = "X-UserId")] long userId,
        [FromHeader(Name = "X-IsAdmin")] bool isAdmin = false,
        [FromHeader(Name = "X-IsModerator")] bool isModerator = false)
    {
        var command = new CreateTopicCommand(
            request.Title,
            userId,
            isAdmin,
            isModerator);

        var result = await _mediator.Send(command);
        return new CreateTopicResponse();
    }

    [HttpPost(nameof(GetLatest))]
    public async Task<GetLatestTopicsResponse> GetLatest(
        [FromBody] GetLatestTopicsRequest request)
    {
        var command = new GetLatestTopicsCommand(request.Page, request.PageSize);

        var results = await _mediator.Send(command);

        return new GetLatestTopicsResponse
        {
            LatestTopics = [.. results.Topics.Select(r => new LatestTopicsResponse
            {
                Id = r.Id,
                Title = r.Title,
                AuthorNikname = r.AuthorNikname,
                CreatedAt = r.CreatedAt
            })],
            TotalCount = results.TotalCount
        };
    }

    [HttpGet("{id:long}/with-comments")]
    public async Task<GetTopicWithCommentsResponse> GetWithComments(
        long id,
        [FromHeader(Name = "X-UserId")] long userId = 0,
        [FromHeader(Name = "X-IsAdmin")] bool isAdmin = false,
        [FromHeader(Name = "X-IsModerator")] bool isModerator = false)
    {
        var result = await _mediator.Send(new GetTopicWithCommentsCommand(id, userId, isAdmin, isModerator));

        return new GetTopicWithCommentsResponse
        {
            TopicId = result.TopicId,
            Title = result.Title,
            AuthorId = result.AuthorId,
            CreatedAt = result.CreatedAt,
            Comments = [.. result.Comments.Select(c => new CommentResponse
            {
                Id = c.Id,
                TopicId = c.TopicId,
                ParentCommentId = c.ParentCommentId,
                AuthorNickname = c.AuthorNickname,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                CanEdit = c.CanEdit,
                CanDelete = c.CanDelete
            })],
            CanEdit = result.CanEdit,
            CanDelete = result.CanDelete
        };
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] UpdateTopicRequest request,
        [FromHeader(Name = "X-UserId")] long userId,
        [FromHeader(Name = "X-IsAdmin")] bool isAdmin = false,
        [FromHeader(Name = "X-IsModerator")] bool isModerator = false)
    {
        var command = new UpdateTopicCommand(
            id,
            request.Title,
            userId,
            isAdmin,
            isModerator);

        await _mediator.Send(command);
        return Ok(new UpdateTopicResponse());
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        long id,
        [FromHeader(Name = "X-UserId")] long userId,
        [FromHeader(Name = "X-IsAdmin")] bool isAdmin = false,
        [FromHeader(Name = "X-IsModerator")] bool isModerator = false)
    {
        var command = new DeleteTopicCommand(id, userId, isAdmin, isModerator);

        await _mediator.Send(command);
        return Ok();
    }
}
