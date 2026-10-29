using CommentsService.Domain.Entities;
using CommentsService.Infrastructure.Data;
using CommentsService.Infrastructure.Repositories.Comments.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CommentsService.Infrastructure.Repositories.Comments;

internal sealed class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CommentRepository> _logger;

    public CommentRepository(
        ApplicationDbContext context,
        ILogger<CommentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> CreateAsync(
        Comment comment,
        CancellationToken cancellationToken)
    {
        try
        {
            await _context.Comments.AddAsync(comment, cancellationToken);
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return -1;
        }
    }

    public async Task<int> UpdateAsync(
        Comment comment,
        CancellationToken cancellationToken)
    {
        try
        {
            _context.Comments.Update(comment);
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return -1;
        }
    }

    public async Task<int> SoftDeleteAsync(
        long commentId,
        DateTimeOffset deletedAt,
        CancellationToken cancellationToken)
    {
        try
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId, cancellationToken);

            if (comment is null)
            {
                return -1;
            }

            comment.SoftDelete(deletedAt);

            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return -1;
        }
    }

    public async Task<PagedCommentsResult> GetByUserIdAsync(
        long userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (pageSize <= 0)
        {
            return new PagedCommentsResult([], 0);
        }

        try
        {
            var totalCount = await _context.Comments
                .CountAsync(c => c.AuthorId == userId && !c.IsDeleted, cancellationToken);

            var results = await _context.Comments
                .Where(c => c.AuthorId == userId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedCommentsResult([.. results], totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении комментариев пользователя {UserId}", userId);
            return new PagedCommentsResult([], 0);
        }
    }

    public async Task<(IReadOnlyCollection<Comment> RootComments, IReadOnlyCollection<Comment> ChildComments)> GetPagedCommentTreeAsync(
        long topicId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (pageSize <= 0)
        {
            return ([], []);
        }

        try
        {
            // 1. Paginate root comments (oldest first)
            var rootComments = await _context.Comments
                .Where(c => c.TopicId == topicId && c.ParentCommentId == null)
                .OrderBy(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            if (!rootComments.Any())
            {
                return ([], []);
            }

            var rootIds = rootComments.Select(rc => rc.Id).ToList();

            // 2. Recursive CTE to fetch all descendants
            var childComments = await _context.Comments
                .FromSqlRaw(@"
                    WITH RECURSIVE CommentTree AS (
                        SELECT id, topic_id, parent_comment_id, author_id, content, created_at, updated_at,
                               updated_by_user_id, is_deleted, deleted_at
                        FROM public.comments
                        WHERE parent_comment_id = ANY({0})

                        UNION ALL

                        SELECT c.id, c.topic_id, c.parent_comment_id, c.author_id, c.content, c.created_at, c.updated_at,
                               c.updated_by_user_id, c.is_deleted, c.deleted_at
                        FROM public.comments c
                        INNER JOIN CommentTree ct ON c.parent_comment_id = ct.id
                    )
                    SELECT * FROM CommentTree", rootIds.ToArray())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return (rootComments.AsReadOnly(), childComments.AsReadOnly());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении древовидных комментариев для топика {TopicId}", topicId);
            return ([], []);
        }
    }
}
