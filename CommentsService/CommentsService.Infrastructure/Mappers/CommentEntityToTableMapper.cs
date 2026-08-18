using CommentsService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommentsService.Infrastructure.Mappers;

internal static class CommentEntityToTableMapper
{
    public static ModelBuilder Map(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("comments", "public");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityAlwaysColumn();

            entity.Property(e => e.TopicId)
                .HasColumnName("topic_id")
                .HasColumnType("bigint")
                .IsRequired();

            entity.Property(e => e.ParentCommentId)
                .HasColumnName("parent_comment_id")
                .HasColumnType("bigint");

            entity.Property(e => e.AuthorId)
                .HasColumnName("author_id")
                .HasColumnType("bigint")
                .IsRequired();

            entity.Property(e => e.Content)
                .HasColumnName("content")
                .HasColumnType("text")
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.HasIndex(e => e.TopicId);
        });

        return modelBuilder;
    }
}
