namespace AuthUsersService.Domain;

/// <summary>
/// Причины отзыва refresh-токена, сохраняемые в <c>RevokedBy</c>.
/// </summary>
public static class RefreshTokenRevokeReason
{
    public const string Refreshed = "refreshed";
    public const string Logout = "logout";
    public const string RevokedOnLogin = "revoked_on_login";
    public const string PasswordChanged = "password_changed";
    public const string ReuseDetected = "reuse_detected";
}
