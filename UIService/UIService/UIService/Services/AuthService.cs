using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace UIService.Services;

public sealed class AuthService
{
    private const string RefreshTokenStorageKey = "auth:refresh";
    private const string RefreshUri = "User/Refresh";
    private const string RevokeUri = "User/Revoke";
    private const string MeUri = "User/Me";
    private const string RefreshClientName = "AuthRefreshClient";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly ILogger<AuthService> _logger;

    private string? _refreshToken;
    private Task<string?>? _refreshTask;

    public AuthService(
        IHttpClientFactory httpClientFactory,
        ProtectedLocalStorage protectedLocalStorage,
        ILogger<AuthService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _protectedLocalStorage = protectedLocalStorage;
        _logger = logger;
    }

    public event Action? OnChanged;

    public long? UserId { get; private set; }
    public string? AccessToken { get; private set; }
    public string? Nickname { get; private set; }
    public bool IsAdmin { get; private set; }
    public bool IsModerator { get; private set; }

    public bool HasRefreshToken => !string.IsNullOrEmpty(_refreshToken);
    public bool IsAuthenticated => UserId.HasValue && UserId > 0 && !string.IsNullOrEmpty(AccessToken);

    /// <summary>
    /// Восстанавливает сессию после перезагрузки страницы: обменивает сохранённый
    /// refresh-токен на новую пару токенов и перечитывает профиль пользователя.
    /// </summary>
    public async Task InitializeAsync()
    {
        if (IsAuthenticated)
        {
            return;
        }

        try
        {
            var stored = await _protectedLocalStorage.GetAsync<string>(RefreshTokenStorageKey);

            if (!stored.Success || string.IsNullOrWhiteSpace(stored.Value))
            {
                return;
            }

            _refreshToken = stored.Value;

            var accessToken = await RefreshAsync();

            if (string.IsNullOrEmpty(accessToken))
            {
                return;
            }

            await RestoreProfileAsync(stored.Value);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось восстановить сессию пользователя.");
        }
    }

    public async Task SetUserAsync(
        long userId,
        string accessToken,
        string? refreshToken,
        string nickname,
        bool isAdmin,
        bool isModerator)
    {
        UserId = userId;
        AccessToken = accessToken;
        Nickname = nickname;
        IsAdmin = isAdmin;
        IsModerator = isModerator;
        _refreshToken = refreshToken;

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await PersistRefreshTokenAsync(refreshToken);
        }

        NotifyChanged();
    }

    /// <summary>
    /// Обновляет access-токен (single-flight: параллельные вызовы ждут один запрос).
    /// Возвращает новый access-токен либо <c>null</c>, если обновление невозможно.
    /// </summary>
    public async Task<string?> RefreshAsync(CancellationToken cancellationToken = default)
    {
        var runningRefresh = _refreshTask;

        if (runningRefresh is not null)
        {
            return await runningRefresh;
        }

        var refreshTask = RefreshInternalAsync(cancellationToken);
        _refreshTask = refreshTask;

        try
        {
            return await refreshTask;
        }
        finally
        {
            _refreshTask = null;
        }
    }

    /// <summary>
    /// Отзывает refresh-токен на сервере и очищает локальное состояние.
    /// </summary>
    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        var refreshToken = _refreshToken;

        try
        {
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                using var client = _httpClientFactory.CreateClient(RefreshClientName);
                using var response = await client.PostAsJsonAsync(
                    RevokeUri,
                    new RefreshRequestBody { RefreshToken = refreshToken },
                    JsonOptions,
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "Не удалось отозвать refresh-токен: {StatusCode}.",
                        response.StatusCode);
                }
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or NotSupportedException)
        {
            _logger.LogWarning(ex, "Ошибка при отзыве refresh-токена.");
        }
        finally
        {
            await ClearAsync();
        }
    }

    /// <summary>
    /// Очищает локальное состояние пользователя и удаляет refresh-токен из хранилища браузера.
    /// </summary>
    public async Task ClearAsync()
    {
        UserId = null;
        AccessToken = null;
        Nickname = null;
        IsAdmin = false;
        IsModerator = false;
        _refreshToken = null;

        try
        {
            await _protectedLocalStorage.DeleteAsync(RefreshTokenStorageKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось удалить refresh-токен из хранилища браузера.");
        }

        NotifyChanged();
    }

    private async Task<string?> RefreshInternalAsync(CancellationToken cancellationToken)
    {
        if (!HasRefreshToken)
        {
            await LoadRefreshTokenAsync();
        }

        if (!HasRefreshToken)
        {
            return null;
        }

        try
        {
            using var client = _httpClientFactory.CreateClient(RefreshClientName);
            using var response = await client.PostAsJsonAsync(
                RefreshUri,
                new RefreshRequestBody { RefreshToken = _refreshToken },
                JsonOptions,
                cancellationToken);

            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                _logger.LogInformation(
                    "Сервер отклонил refresh-токен ({StatusCode}); сессия очищается.",
                    response.StatusCode);

                await ClearAsync();
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Не удалось обновить токен доступа: {StatusCode}.",
                    response.StatusCode);

                return null;
            }

            var payload = await response.Content.ReadFromJsonAsync<RefreshPayload>(JsonOptions, cancellationToken);

            if (payload is null
                || string.IsNullOrWhiteSpace(payload.Token)
                || string.IsNullOrWhiteSpace(payload.RefreshToken))
            {
                await ClearAsync();
                return null;
            }

            AccessToken = payload.Token;
            _refreshToken = payload.RefreshToken;

            await PersistRefreshTokenAsync(payload.RefreshToken);
            NotifyChanged();

            return payload.Token;
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or NotSupportedException)
        {
            _logger.LogWarning(ex, "Ошибка при обновлении токена доступа.");
            return null;
        }
    }

    private async Task RestoreProfileAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = _httpClientFactory.CreateClient(RefreshClientName);
            using var response = await client.PostAsJsonAsync(
                MeUri,
                new RefreshRequestBody { RefreshToken = refreshToken },
                JsonOptions,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Не удалось получить профиль пользователя: {StatusCode}.",
                    response.StatusCode);

                await ClearAsync();
                return;
            }

            var profile = await response.Content.ReadFromJsonAsync<MePayload>(JsonOptions, cancellationToken);

            if (profile is null || !profile.IsAuthenticated || profile.UserId is null)
            {
                await ClearAsync();
                return;
            }

            UserId = profile.UserId;
            Nickname = profile.Nickname;
            IsAdmin = profile.IsAdmin;
            IsModerator = profile.IsModerator;

            NotifyChanged();
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or NotSupportedException)
        {
            _logger.LogWarning(ex, "Ошибка при получении профиля пользователя.");
        }
    }

    private async Task LoadRefreshTokenAsync()
    {
        try
        {
            var stored = await _protectedLocalStorage.GetAsync<string>(RefreshTokenStorageKey);

            if (stored.Success && !string.IsNullOrWhiteSpace(stored.Value))
            {
                _refreshToken = stored.Value;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось прочитать refresh-токен из хранилища браузера.");
        }
    }

    private async Task PersistRefreshTokenAsync(string refreshToken)
    {
        try
        {
            await _protectedLocalStorage.SetAsync(RefreshTokenStorageKey, refreshToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось сохранить refresh-токен в хранилище браузера.");
        }
    }

    private void NotifyChanged()
    {
        OnChanged?.Invoke();
    }
}

internal sealed class RefreshRequestBody
{
    [JsonPropertyName("refreshToken")]
    public string? RefreshToken { get; set; }
}

internal sealed class RefreshPayload
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("refreshToken")]
    public string? RefreshToken { get; set; }
}

internal sealed class MePayload
{
    [JsonPropertyName("isAuthenticated")]
    public bool IsAuthenticated { get; set; }

    [JsonPropertyName("userId")]
    public long? UserId { get; set; }

    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    [JsonPropertyName("isAdmin")]
    public bool IsAdmin { get; set; }

    [JsonPropertyName("isModerator")]
    public bool IsModerator { get; set; }
}
