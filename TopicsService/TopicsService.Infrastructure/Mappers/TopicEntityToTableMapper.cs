using Microsoft.EntityFrameworkCore;
using TopicsService.Domain.Entities;
using TopicsService.Infrastructure.Schemas;

namespace TopicsService.Infrastructure.Mappers;

internal static class TopicEntityToTableMapper
{
    public static ModelBuilder Map(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Topic>(topic =>
        {
            topic.ToTable(
                TopicsTable.TableName,
                TopicsTable.SchemaName);

            topic.HasKey(t => t.Id);

            topic.Property(t => t.Id)
                .HasColumnName(TopicsTable.IdName)
                .UseIdentityAlwaysColumn();

            topic.Property(t => t.Title)
                .HasColumnName(TopicsTable.TitleName)
                .HasColumnType("text")
                .IsRequired();

            topic.Property(t => t.AuthorId)
                .HasColumnName(TopicsTable.AuthorIdName)
                .HasColumnType("bigint")
                .IsRequired();

            topic.Property(u => u.CreatedAt)
                .HasColumnName(TopicsTable.CreatedAtName)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            topic.Property(u => u.UpdatedAt)
                .HasColumnName(TopicsTable.UpdatedAtName)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            topic.Property(t => t.IsDeleted)
                .HasColumnName(TopicsTable.IsDeletedName)
                .HasColumnType("boolean")
                .IsRequired();

            topic.Property(u => u.DeletedAt)
                .HasColumnName(TopicsTable.DeletedAtName)
                .HasColumnType("timestamp with time zone");

            topic.HasIndex(t => new { t.IsDeleted, t.CreatedAt });
        });

        return modelBuilder;
    }
}
