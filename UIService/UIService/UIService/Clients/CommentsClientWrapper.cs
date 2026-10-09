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

    public Task<CreateCommentResponse> CreateAsync(long? userId, CreateCommentRequest body)
        => _inner.CreateAsync(userId, body);

    public Task<CreateCommentResponse> CreateAsync(long? userId, CreateCommentRequest body, CancellationToken cancellationToken)
        => _inner.CreateAsync(userId, body, cancellationToken);

    public Task<UpdateCommentResponse> UpdateAsync(long? id, long? x_UserId, UpdateCommentRequest body)
        => _inner.UpdateAsync(id, x_UserId, body);

    public Task<UpdateCommentResponse> UpdateAsync(long? id, long? x_UserId, UpdateCommentRequest body, CancellationToken cancellationToken)
        => _inner.UpdateAsync(id, x_UserId, body, cancellationToken);

    public Task<DeleteCommentResponse> DeleteAsync(long? id)
        => _inner.DeleteAsync(id);

    public Task<DeleteCommentResponse> DeleteAsync(long? id, CancellationToken cancellationToken)
        => _inner.DeleteAsync(id, cancellationToken);
}
