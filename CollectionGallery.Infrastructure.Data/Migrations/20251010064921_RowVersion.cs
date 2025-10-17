using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectionGallery.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE IF EXISTS models
                ADD COLUMN IF NOT EXISTS _row_version bytea NOT NULL
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
