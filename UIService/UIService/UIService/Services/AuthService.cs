using Microsoft.JSInterop;

namespace UIService.Services;

public class AuthService
{
    private readonly IJSRuntime _jsRuntime;
    private long? _userId;
    private string? _token;
    private string? _nickname;

    public event Action? OnChanged;

    public long? UserId => _userId;
    public string? Token => _token;
    public string? Nickname => _nickname;
    public bool IsAuthenticated => _userId.HasValue && _userId > 0;

    public AuthService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync(IJSRuntime jsRuntime)
    {
        try
        {
            var userId = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authUserId");
            var token = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            var nickname = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authNickname");

            if (!string.IsNullOrEmpty(userId) && long.TryParse(userId, out var parsedUserId))
            {
                _userId = parsedUserId;
                _token = token;
                _nickname = nickname;
            }
        }
        catch
        {
            // If localStorage is not available, start with empty state
        }
    }

    public async Task SetUserAsync(long userId, string token, string nickname)
    {
        _userId = userId;
        _token = token;
        _nickname = nickname;

        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authUserId", userId.ToString());
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authNickname", nickname);

        OnChanged?.Invoke();
    }

    public async Task ClearAsync()
    {
        _userId = null;
        _token = null;
        _nickname = null;

        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authUserId");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authNickname");

        OnChanged?.Invoke();
    }
}
