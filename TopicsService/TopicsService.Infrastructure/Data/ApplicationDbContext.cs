using Microsoft.EntityFrameworkCore;
using TopicsService.Domain.Entities;
using TopicsService.Infrastructure.Mappers;

namespace TopicsService.Infrastructure.Data;

internal sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Topic> Topics => Set<Topic>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        TopicEntityToTableMapper.Map(modelBuilder);
    }
}
