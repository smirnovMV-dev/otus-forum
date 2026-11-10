using CommentsService.Infrastructure.Repositories.Comments;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CommentsService.Application.Comments.CountComments;

public sealed record CountCommentsCommand() : IRequest<int>;

internal sealed class CountCommentsCommandHandler : IRequestHandler<CountCommentsCommand, int>
{
    private readonly ICommentRepository _commentRepository;

    public CountCommentsCommandHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<int> Handle(
        CountCommentsCommand request,
        CancellationToken cancellationToken)
    {
        return await _commentRepository.CountAsync(cancellationToken);
    }
}
