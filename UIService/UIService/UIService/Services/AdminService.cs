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
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly IUsersClient _usersClient;

    public AdminService(HttpClient httpClient, AuthService authService, IUsersClient usersClient)
    {
        _httpClient = httpClient;
        _authService = authService;
        _usersClient = usersClient;
    }

    private void ApplyAuthHeader(HttpRequestMessage request)
    {
        var token = _authService.Token;
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        request.Headers.Add("X-IsAdmin", _authService.IsAdmin.ToString().ToLowerInvariant());
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IReadOnlyCollection<UserSearchModel>> SearchUsersAsync(string nicknamePart)
    {
        var uri = new Uri($"User/SearchByNickname?nicknamePart={Uri.EscapeDataString(nicknamePart)}", UriKind.Relative);
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        ApplyAuthHeader(request);
        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IReadOnlyCollection<UserSearchModel>>(content, JsonOptions) ?? [];
    }

    public async Task<UserWithRolesModel> GetUserWithRolesAsync(long userId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"User/GetWithRoles/{userId}");
        ApplyAuthHeader(request);
        var response = await _httpClient.SendAsync(request);
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
        var uri = new Uri("Role/GetAll", UriKind.Relative);
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        ApplyAuthHeader(request);
        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        var rolesResponse = JsonSerializer.Deserialize<GetAllRolesResponse>(content, JsonOptions);
        return rolesResponse?.Roles ?? [];
    }
}
