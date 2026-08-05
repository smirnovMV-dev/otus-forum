using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TopicsService.API.Models.Topics.CreateTopic;
using TopicsService.API.Models.Topics.GetLatestTopics;
using TopicsService.Application.Topics.CreateTopic;
using TopicsService.Application.Topics.GetLatestTopics;

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
        [FromBody] CreateTopicRequest request)
    {
        var command = new CreateTopicCommand(
            request.Title,
            request.AuthorId);

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
}
