using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TopicsService.API.Models.Topics.CreateTopic;
using TopicsService.Application.Topics.CreateTopic;

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
            request.Caption,
            request.AuthorId);

        var result = await _mediator.Send(command);
        return new CreateTopicResponse();
    }
}
