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
            migrationBuilder.AddColumn<long?>(
                name: "updated_by_user_id",
                schema: "public",
                table: "comments",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "updated_by_user_id",
                schema: "public",
                table: "comments");
        }
    }
}
