namespace UIService.Clients;

using OtusForum.UI.Clients.Topics;

public class TopicsClientWrapper : ITopicsClient
{
    private readonly ITopicsClient _inner;

    public TopicsClientWrapper(IHttpClientFactory factory, IConfiguration configuration)
    {
        var apiUrl = configuration.GetValue<string>("ApiUrls:Topics") ?? "http://topics-service:5294";
        var httpClient = factory.CreateClient();
        httpClient.BaseAddress = new Uri(apiUrl);
        _inner = new TopicsClient(httpClient) { BaseUrl = apiUrl.TrimEnd('/') };
    }

    public Task<CreateTopicResponse> CreateAsync(long? userId, CreateTopicRequest body)
        => _inner.CreateAsync(userId, body);

    public Task<CreateTopicResponse> CreateAsync(long? userId, CreateTopicRequest body, CancellationToken cancellationToken)
        => _inner.CreateAsync(userId, body, cancellationToken);

    public Task<GetLatestTopicsResponse> GetLatestAsync(GetLatestTopicsRequest body)
        => _inner.GetLatestAsync(body);

    public Task<GetLatestTopicsResponse> GetLatestAsync(GetLatestTopicsRequest body, CancellationToken cancellationToken)
        => _inner.GetLatestAsync(body, cancellationToken);

    public Task<GetTopicWithCommentsResponse> WithCommentsAsync(long id)
        => _inner.WithCommentsAsync(id);

    public Task<GetTopicWithCommentsResponse> WithCommentsAsync(long id, CancellationToken cancellationToken)
        => _inner.WithCommentsAsync(id, cancellationToken);

    public Task TopicPUTAsync(long id, long? userId, UpdateTopicRequest body)
        => _inner.TopicPUTAsync(id, userId, body);

    public Task TopicPUTAsync(long id, long? userId, UpdateTopicRequest body, CancellationToken cancellationToken)
        => _inner.TopicPUTAsync(id, userId, body, cancellationToken);

    public Task TopicDELETEAsync(long id, long? userId)
        => _inner.TopicDELETEAsync(id, userId);

    public Task TopicDELETEAsync(long id, long? userId, CancellationToken cancellationToken)
        => _inner.TopicDELETEAsync(id, userId, cancellationToken);
}
