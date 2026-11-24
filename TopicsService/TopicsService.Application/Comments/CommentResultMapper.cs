using System;
using System.Collections.Generic;
using System.Linq;
using TopicsService.Application.Services.Externals.CommentsApiService.Models;
using TopicsService.Application.Services.Externals.UsersApiService.Models;
using TopicsService.Application.Topics.GetTopicWithComments.Models;

namespace TopicsService.Application.Comments;

internal static class CommentResultMapper
{
    internal static CommentResult Map(
        CommentDto dto,
        IReadOnlyDictionary<long, UserNickname> nicknames,
        long userId,
        bool isAdmin,
        bool isModerator)
    {
        var canEdit = userId == dto.AuthorId || isAdmin || isModerator;
        var canDelete = userId == dto.AuthorId || isAdmin;

        return new CommentResult(
            dto.Id,
            dto.TopicId,
            dto.ParentCommentId,
            nicknames.TryGetValue(dto.AuthorId, out var nickname) ? nickname.Nickname : "Неизвестный пользователь",
            dto.Content,
            DateTimeOffset.Parse(dto.CreatedAt),
            canEdit,
            canDelete,
            dto.Replies.Select(r => Map(r, nicknames, userId, isAdmin, isModerator)).ToList(),
            dto.RepliesTotalCount);
    }
}
