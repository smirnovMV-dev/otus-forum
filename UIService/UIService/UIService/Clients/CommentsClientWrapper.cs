namespace UIService.Clients;

using OtusForum.UI.Clients.Comments;

public class CommentsClientWrapper : ICommentsClient
{
    private readonly ICommentsClient _inner;

    public CommentsClientWrapper(IHttpClientFactory factory, IConfiguration configuration)
    {
        var apiUrl = configuration.GetValue<string>("ApiUrls:Comments") ?? "http://comments-service:5044";
        var httpClient = factory.CreateClient("CommentsClient");
        _inner = new CommentsClient(httpClient) { BaseUrl = apiUrl.TrimEnd('/') };
    }

    public Task<CreateCommentResponse> CreateAsync(CreateCommentRequest body)
        => _inner.CreateAsync(body);

    public Task<CreateCommentResponse> CreateAsync(CreateCommentRequest body, CancellationToken cancellationToken)
        => _inner.CreateAsync(body, cancellationToken);
}
