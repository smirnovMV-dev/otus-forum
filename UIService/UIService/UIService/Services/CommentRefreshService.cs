using System;

namespace UIService.Services;

public class CommentRefreshService
{
    public event Func<Task> OnCommentChanged = () => Task.CompletedTask;

    public async Task NotifyCommentAdded()
    {
        await OnCommentChanged();
    }
}
