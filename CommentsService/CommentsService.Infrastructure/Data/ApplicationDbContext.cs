using CommentsService.Domain.Entities;
using CommentsService.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace CommentsService.Infrastructure.Data;

internal sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        CommentEntityToTableMapper.Map(modelBuilder);
    }
}
