using System.Net.Http.Headers;
using System.Text.Json;
using OtusForum.UI.Clients.Users;

namespace UIService.Services;

public interface IAdminService
{
    Task<IReadOnlyCollection<UserSearchModel>> SearchUsersAsync(string nicknamePart);
    Task<UserWithRolesModel> GetUserWithRolesAsync(long userId);
    Task AssignRoleAsync(long userId, long roleId, DateTimeOffset? expiresAt = null);
    Task RemoveRoleAsync(long userId, long roleId);
    Task<IReadOnlyCollection<RoleModel>> GetAllRolesAsync();
}

public class AdminService : IAdminService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthService _authService;
    private readonly IUsersClient _usersClient;

    public AdminService(IHttpClientFactory httpClientFactory, AuthService authService, IUsersClient usersClient)
    {
        _httpClientFactory = httpClientFactory;
        _authService = authService;
        _usersClient = usersClient;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private HttpClient CreateAuthenticatedClient()
    {
        var client = _httpClientFactory.CreateClient("UsersClient");
        // The AuthHeaderHandler will add the Bearer token
        return client;
    }

    public async Task<IReadOnlyCollection<UserSearchModel>> SearchUsersAsync(string nicknamePart)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.GetAsync($"User/SearchByNickname?nicknamePart={Uri.EscapeDataString(nicknamePart)}");
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IReadOnlyCollection<UserSearchModel>>(content, JsonOptions) ?? [];
    }

    public async Task<UserWithRolesModel> GetUserWithRolesAsync(long userId)
    {
        var client = CreateAuthenticatedClient();
        var response = await client.GetAsync($"User/GetWithRoles/{userId}");
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserWithRolesModel>(content, JsonOptions)
            ?? throw new Exception("Failed to load user with roles");
    }

    public async Task AssignRoleAsync(long userId, long roleId, DateTimeOffset? expiresAt = null)
    {
        var request = new AssignUserRoleRequest { UserId = userId, RoleId = roleId, ExpiresAt = expiresAt };
        await _usersClient.AssignRoleAsync(request);
    }

    public async Task RemoveRoleAsync(long userId, long roleId)
    {
        var request = new RemoveUserRoleRequest { UserId = userId, RoleId = roleId };
        await _usersClient.RemoveRoleAsync(true, request);
    }

    public async Task<IReadOnlyCollection<RoleModel>> GetAllRolesAsync()
    {
        var client = CreateAuthenticatedClient();
        var response = await client.GetAsync("Role/GetAll");
        var content = await response.Content.ReadAsStringAsync();
        var rolesResponse = JsonSerializer.Deserialize<GetAllRolesResponse>(content, JsonOptions);
        return rolesResponse?.Roles ?? [];
    }
}
