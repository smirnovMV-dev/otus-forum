using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.JSInterop;

namespace UIService.Services;

public class AuthService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<AuthService> _logger;
    private long? _userId;
    private string? _accessToken;
    private string? _nickname;
    private bool _isAdmin;
    private bool _isModerator;

    public event Action? OnChanged;

    public long? UserId => _userId;
    public string? AccessToken => _accessToken;
    public string? Nickname => _nickname;
    public bool IsAuthenticated => _userId.HasValue && _userId > 0 && !string.IsNullOrEmpty(_accessToken);
    public bool IsAdmin => _isAdmin;
    public bool IsModerator => _isModerator;

    public AuthService(IJSRuntime jsRuntime, ILogger<AuthService> logger)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var userId = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authUserId");
            var nickname = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authNickname");
            var isAdmin = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authIsAdmin");
            var isModerator = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authIsModerator");

            if (!string.IsNullOrEmpty(userId) && long.TryParse(userId, out var parsedUserId))
            {
                _userId = parsedUserId;
                _nickname = nickname;
                _isAdmin = bool.TryParse(isAdmin, out var parsedAdmin) && parsedAdmin;
                _isModerator = bool.TryParse(isModerator, out var parsedModerator) && parsedModerator;
                
                // Token will be set when user logs in or when refresh happens
                // If we have user data but no token, the user might have been logged out on server side
                // AuthHeaderHandler will handle 401 and clear state
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось инициализировать авторизацию");
        }
    }

    public async Task SetUserAsync(long userId, string accessToken, string nickname, bool isAdmin = false, bool isModerator = false)
    {
        // Validate JWT expiration before setting
        if (!string.IsNullOrEmpty(accessToken) && IsJwtExpired(accessToken))
        {
            _logger.LogWarning("Access token is already expired, not setting user");
            return;
        }

        _userId = userId;
        _accessToken = accessToken;
        _nickname = nickname;
        _isAdmin = isAdmin;
        _isModerator = isModerator;

        // Save user info to localStorage (without token)
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authUserId", userId.ToString());
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authNickname", nickname);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authIsAdmin", isAdmin.ToString());
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authIsModerator", isModerator.ToString());

        OnChanged?.Invoke();
    }

    public async Task ClearAsync()
    {
        _userId = null;
        _accessToken = null;
        _nickname = null;
        _isAdmin = false;
        _isModerator = false;

        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authUserId");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authNickname");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authIsAdmin");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authIsModerator");

        OnChanged?.Invoke();
    }

    public void UpdateAccessToken(string newToken)
    {
        _accessToken = newToken;
        OnChanged?.Invoke();
    }

    private bool IsJwtExpired(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return false;

            var payload = Base64UrlDecode(parts[1]);
            using var jsonDoc = JsonDocument.Parse(payload);
            var root = jsonDoc.RootElement;
            
            if (root.TryGetProperty("exp", out var expProperty))
            {
                var expTimestamp = expProperty.GetInt64();
                var expDate = DateTimeOffset.FromUnixTimeSeconds(expTimestamp);
                return expDate < DateTimeOffset.UtcNow;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось проверить expiration JWT токена");
        }
        
        return false;
    }

    private static string Base64UrlDecode(string base64Url)
    {
        var base64 = base64Url.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
    }
}
