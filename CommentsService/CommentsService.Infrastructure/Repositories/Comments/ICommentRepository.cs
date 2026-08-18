using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommentsService.Domain.Entities;

namespace CommentsService.Infrastructure.Repositories.Comments;

public interface ICommentRepository
{
    Task<int> CreateAsync(
        Comment comment,
        CancellationToken cancellationToken);
}
