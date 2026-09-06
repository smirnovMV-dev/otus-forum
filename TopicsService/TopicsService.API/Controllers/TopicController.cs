using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TopicsService.API.Models.Topics.CreateTopic;
using TopicsService.API.Models.Topics.GetLatestTopics;
using TopicsService.API.Models.Topics.GetTopicWithComments;
using TopicsService.Application.Topics.CreateTopic;
using TopicsService.Application.Topics.GetLatestTopics;
using TopicsService.Application.Topics.GetTopicWithComments;

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
        [FromHeader(Name = "X-UserId")] long userId)
    {
        var command = new CreateTopicCommand(
            request.Title,
            userId);

        var result = await _mediator.Send(command);
        return new CreateTopicResponse();
    }

    [HttpPost(nameof(GetLatest))]
    public async Task<GetLatestTopicsResponse> GetLatest(
        [FromBody] GetLatestTopicsRequest request)
    {
        var command = new GetLatestTopicsCommand();

        var results = await _mediator.Send(command);

        return new GetLatestTopicsResponse
        {
            LatestTopics = [.. results.Select(r => new LatestTopicsResponse
            {
                Id = r.Id,
                Title = r.Title,
                AuthorNikname = r.AuthorNikname,
                CreatedAt = r.CreatedAt
            }
            )],
        };
    }

    [HttpGet("{id:long}/with-comments")]
    public async Task<GetTopicWithCommentsResponse> GetWithComments(long id)
    {
        var result = await _mediator.Send(new GetTopicWithCommentsCommand(id));

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
                CreatedAt = c.CreatedAt
            })]
        };
    }
}
