namespace UIService.Services;

public class AuthService
{
    private long? _userId;
    private string? _token;
    private string? _nickname;

    public event Action? OnChanged;

    public long? UserId => _userId;
    public string? Token => _token;
    public string? Nickname => _nickname;
    public bool IsAuthenticated => _userId.HasValue && _userId > 0;

    public void SetUser(long userId, string token, string nickname)
    {
        _userId = userId;
        _token = token;
        _nickname = nickname;
        OnChanged?.Invoke();
    }

    public void Clear()
    {
        _userId = null;
        _token = null;
        _nickname = null;
        OnChanged?.Invoke();
    }
}
