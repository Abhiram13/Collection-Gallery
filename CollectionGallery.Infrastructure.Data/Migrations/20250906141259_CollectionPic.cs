using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectionGallery.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CollectionPic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE IF EXISTS collections
                ADD COLUMN IF NOT EXISTS collection_pic VARCHAR
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {            
        }
    }
}
