using System.Net;
using System.Net.Http.Headers;
using UIService.Services;

namespace UIService.Handlers;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly AuthService _authService;

    public AuthHeaderHandler(AuthService authService)
    {
        _authService = authService;
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

        if (response.StatusCode == HttpStatusCode.Unauthorized && _authService.HasRefreshToken)
        {
            var refreshedToken = await _authService.RefreshAsync(cancellationToken);

            if (!string.IsNullOrEmpty(refreshedToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshedToken);
                response = await base.SendAsync(request, cancellationToken);
            }
        }

        return response;
    }
}
