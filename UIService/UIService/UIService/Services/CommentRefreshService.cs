using System;

namespace UIService.Services;

public class CommentRefreshService
{
    public event Action? OnCommentChanged;

    public void NotifyCommentAdded()
    {
        OnCommentChanged?.Invoke();
    }
}
