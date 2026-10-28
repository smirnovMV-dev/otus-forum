using System.Collections.Generic;
using CommentsService.Domain.Entities;

namespace CommentsService.Infrastructure.Repositories.Comments.Models;

public sealed record PagedCommentsResult(
    IReadOnlyCollection<Comment> Comments,
    int TotalCount);
