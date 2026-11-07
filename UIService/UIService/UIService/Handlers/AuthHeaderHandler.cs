using System.Net.Http.Headers;
using Microsoft.JSInterop;
using UIService.Services;

namespace UIService.Handlers;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly AuthService _authService;
    private readonly IJSRuntime _jsRuntime;
    private readonly SemaphoreSlim _refreshSemaphore = new(1, 1);
    private bool _isRefreshing;

    public AuthHeaderHandler(AuthService authService, IJSRuntime jsRuntime)
    {
        _authService = authService;
        _jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _authService.AccessToken;
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // Try to refresh token - prevent concurrent refreshes
            if (!_isRefreshing)
            {
                await _refreshSemaphore.WaitAsync(cancellationToken);
                try
                {
                    if (!_isRefreshing)
                    {
                        _isRefreshing = true;
                        await TryRefreshAsync();
                        _isRefreshing = false;

                        // If refresh succeeded, retry the original request
                        if (_authService.IsAuthenticated && !string.IsNullOrEmpty(_authService.AccessToken))
                        {
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authService.AccessToken);
                            response = await base.SendAsync(request, cancellationToken);
                        }
                    }
                }
                finally
                {
                    _refreshSemaphore.Release();
                }
            }
        }

        return response;
    }

    private async Task TryRefreshAsync()
    {
        try
        {
            // Call refresh endpoint - httpOnly cookie is sent automatically
            var refreshResponse = await _jsRuntime.InvokeAsync<string>(
                "fetch", "/api/auth/refresh", new { method = "POST" });

            // Parse the response to get new token
            // The fetch returns JSON: { token: "new_jwt_token" }
            // We need to parse it
            var jsonStart = refreshResponse.IndexOf('{');
            var jsonEnd = refreshResponse.LastIndexOf('}');
            
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var json = refreshResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);
                var tokenStart = json.IndexOf("\"token\"");
                if (tokenStart >= 0)
                {
                    var tokenValueStart = json.IndexOf(':', tokenStart) + 1;
                    var tokenValueEnd = json.IndexOf('"', tokenValueStart + 7);
                    if (tokenValueEnd > tokenValueStart)
                    {
                        var newToken = json.Substring(tokenValueStart + 2, tokenValueEnd - tokenValueStart - 2).Trim();
                        if (!string.IsNullOrEmpty(newToken))
                        {
                            // Update access token in AuthService
                            // We need to call SetUserAsync but we don't have userId/nickname here
                            // Alternative: store token directly
                            _authService.UpdateAccessToken(newToken);
                        }
                    }
                }
            }
        }
        catch
        {
            // Refresh failed - clear auth state
            await _authService.ClearAsync();
        }
    }
}
