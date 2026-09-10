namespace UIService.Clients;

using OtusForum.UI.Clients.Users;

public class UsersClientWrapper : IUsersClient
{
    private readonly IUsersClient _inner;

    public UsersClientWrapper(IHttpClientFactory factory, IConfiguration configuration)
    {
        var apiUrl = configuration.GetValue<string>("ApiUrls:Auth") ?? "http://auth-users-service:5225";
        var httpClient = factory.CreateClient("UsersClient");
        _inner = new UsersClient(httpClient) { BaseUrl = apiUrl.TrimEnd('/') };
    }

    public Task<CreateRoleResponse> CreateAsync(CreateRoleRequest body)
        => _inner.CreateAsync(body);

    public Task<CreateRoleResponse> CreateAsync(CreateRoleRequest body, CancellationToken cancellationToken)
        => _inner.CreateAsync(body, cancellationToken);

    public Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest body)
        => _inner.RegisterAsync(body);

    public Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest body, CancellationToken cancellationToken)
        => _inner.RegisterAsync(body, cancellationToken);

    public Task<AssignUserRoleResponse> AssignRoleAsync(AssignUserRoleRequest body)
        => _inner.AssignRoleAsync(body);

    public Task<AssignUserRoleResponse> AssignRoleAsync(AssignUserRoleRequest body, CancellationToken cancellationToken)
        => _inner.AssignRoleAsync(body, cancellationToken);

    public Task<LoginResponse> LoginAsync(LoginRequest body)
        => _inner.LoginAsync(body);

    public Task<LoginResponse> LoginAsync(LoginRequest body, CancellationToken cancellationToken)
        => _inner.LoginAsync(body, cancellationToken);

    public Task<int> CountUsersAsync()
        => _inner.CountUsersAsync();

    public Task<int> CountUsersAsync(CancellationToken cancellationToken)
        => _inner.CountUsersAsync(cancellationToken);

    public Task<int> CountRolesAsync()
        => _inner.CountRolesAsync();

    public Task<int> CountRolesAsync(CancellationToken cancellationToken)
        => _inner.CountRolesAsync(cancellationToken);
}
