using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopicsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedCreatedAtIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_topics_is_deleted_created_at",
                schema: "public",
                table: "topics",
                columns: new[] { "is_deleted", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_topics_is_deleted_created_at",
                schema: "public",
                table: "topics");
        }
    }
}
