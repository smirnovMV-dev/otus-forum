using Microsoft.Extensions.Logging;

namespace UIService.Handlers;

public class ErrorLoggingHandler : DelegatingHandler
{
    private readonly ILogger<ErrorLoggingHandler> _logger;

    public ErrorLoggingHandler(ILogger<ErrorLoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogError(
                "Ошибка HTTP-запроса: {Method} {Uri} вернул {StatusCode}. Ответ: {Content}",
                request.Method,
                request.RequestUri,
                response.StatusCode,
                content);
        }

        return response;
    }
}
