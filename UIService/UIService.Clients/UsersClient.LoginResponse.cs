namespace OtusForum.UI.Clients.Users;

/// <summary>
/// Дополнение к сгенерированному контракту до перегенерации NSwag-клиентов:
/// сервер возвращает refresh-токен в теле ответа Login.
/// </summary>
public partial class LoginResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("refreshToken")]
    public string? RefreshToken { get; set; }
}
