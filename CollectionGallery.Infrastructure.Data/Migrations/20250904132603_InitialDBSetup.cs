using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CollectionGallery.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialDBSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS collections (
                    id SERIAL NOT NULL PRIMARY KEY,
                    name VARCHAR NOT NULL,
                    parent_collection_id INT,
                    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                    updated_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                    FOREIGN KEY (parent_collection_id) REFERENCES collections(id)
                )
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS models (
                    id SERIAL NOT NULL PRIMARY KEY,
                    name VARCHAR NOT NULL,                    
                    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                    updated_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW()
                )
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS items (
                    id SERIAL NOT NULL PRIMARY KEY,
                    name VARCHAR NOT NULL,
                    extension VARCHAR (20),
                    model_id int,
                    parent_collection_id INT,
                    size int,
                    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                    updated_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                    FOREIGN KEY (parent_collection_id) REFERENCES collections(id),
                    FOREIGN KEY (model_id) REFERENCES models(id)
                )
            ");            

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS platforms (
                    id SERIAL NOT NULL PRIMARY KEY,
                    name VARCHAR NOT NULL,
                    icon VARCHAR,
                    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                    updated_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW()
                )
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS tags (
                    id SERIAL NOT NULL PRIMARY KEY,
                    name VARCHAR NOT NULL,
                    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
                    updated_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW()
                )
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS itemTags (
                    item_id INT NOT NULL,
                    tag_id INT NOT NULL,
                    FOREIGN KEY (item_id) REFERENCES items(id),
                    FOREIGN KEY (tag_id) REFERENCES tags(id)
                )
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS itemPlatforms (
                    item_id INT NOT NULL,
                    platform_id INT NOT NULL,
                    FOREIGN KEY (item_id) REFERENCES items(id),
                    FOREIGN KEY (platform_id) REFERENCES platforms(id)
                )
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {            
        }
    }
}
