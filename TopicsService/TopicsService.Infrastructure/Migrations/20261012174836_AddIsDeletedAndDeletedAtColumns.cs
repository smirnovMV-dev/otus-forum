using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopicsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedAndDeletedAtColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                schema: "public",
                table: "topics",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                schema: "public",
                table: "topics",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "deleted_at",
                schema: "public",
                table: "topics");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                schema: "public",
                table: "topics");
        }
    }
}
