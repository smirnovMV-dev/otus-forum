using System.Collections.Generic;

namespace TopicsService.Application.Services.Externals.CommentsApiService.Models;

public sealed record PagedCommentsDto(
    IReadOnlyCollection<CommentDto> Comments,
    int TotalCount);
