using CommentsService.Domain.Entities;
using CommentsService.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System;
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
}
