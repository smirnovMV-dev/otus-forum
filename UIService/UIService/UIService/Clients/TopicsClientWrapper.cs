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

    public Task<CreateTopicResponse> CreateAsync(CreateTopicRequest body)
        => _inner.CreateAsync(body);

    public Task<CreateTopicResponse> CreateAsync(CreateTopicRequest body, CancellationToken cancellationToken)
        => _inner.CreateAsync(body, cancellationToken);

    public Task<GetLatestTopicsResponse> GetLatestAsync(GetLatestTopicsRequest body)
        => _inner.GetLatestAsync(body);

    public Task<GetLatestTopicsResponse> GetLatestAsync(GetLatestTopicsRequest body, CancellationToken cancellationToken)
        => _inner.GetLatestAsync(body, cancellationToken);
}
