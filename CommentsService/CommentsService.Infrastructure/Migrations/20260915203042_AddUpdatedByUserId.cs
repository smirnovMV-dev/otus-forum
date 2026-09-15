using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedByUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "updated_by_author_id",
                schema: "public",
                table: "comments",
                newName: "updated_by_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "updated_by_user_id",
                schema: "public",
                table: "comments",
                newName: "updated_by_author_id");
        }
    }
}
