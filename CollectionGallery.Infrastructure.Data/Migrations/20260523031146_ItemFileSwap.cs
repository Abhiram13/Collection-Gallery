using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectionGallery.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ItemFileSwap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_files_file_id",
                table: "items");

            migrationBuilder.DropIndex(
                name: "IX_items_file_id",
                table: "items");

            migrationBuilder.DropColumn(
                name: "file_id",
                table: "items");

            migrationBuilder.AddColumn<int>(
                name: "item_id",
                table: "files",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_files_item_id",
                table: "files",
                column: "item_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_files_items_item_id",
                table: "files",
                column: "item_id",
                principalTable: "items",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_files_items_item_id",
                table: "files");

            migrationBuilder.DropIndex(
                name: "IX_files_item_id",
                table: "files");

            migrationBuilder.DropColumn(
                name: "item_id",
                table: "files");

            migrationBuilder.AddColumn<int>(
                name: "file_id",
                table: "items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_items_file_id",
                table: "items",
                column: "file_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_items_files_file_id",
                table: "items",
                column: "file_id",
                principalTable: "files",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
