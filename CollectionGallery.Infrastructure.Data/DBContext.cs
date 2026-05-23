using CollectionGallery.InfraStructure.Data;
using Microsoft.EntityFrameworkCore;
using CollectionGallery.InfraStructure.Data.Entities;
using Microsoft.EntityFrameworkCore.Design;
using CollectionTable = CollectionGallery.InfraStructure.Data.Constants.DbTableNames.Collection;
using ItemTable = CollectionGallery.InfraStructure.Data.Constants.DbTableNames.Item;
using FileTable = CollectionGallery.InfraStructure.Data.Constants.DbTableNames.File;
using TagTable = CollectionGallery.InfraStructure.Data.Constants.DbTableNames.Tag;
using ItemTagTable = CollectionGallery.InfraStructure.Data.Constants.DbTableNames.ItemTag;

namespace CollectionGallery.InfraStructure.Data;

public abstract class BaseDbContext<TContext> : DbContext where TContext : DbContext
{
    public BaseDbContext(DbContextOptions<TContext> options) : base(options) { }

    public DbSet<CollectionEntity> Collections { get; init; }
    public DbSet<ItemEntity> Items { get; init; }
    public DbSet<CollectionFile> CollectionFiles { get; init; }
    public DbSet<Tags> Tags { get; init; }
    public DbSet<ItemTags> ItemTags { get; init; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CollectionEntity>(entity =>
        {
            entity.ToTable(CollectionTable.TABLE_NAME).HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(255);
            entity.Property(c => c.CreatedAt).IsRequired();
            entity.Property(c => c.UpdatedAt).IsRequired();

            entity.HasOne(c => c.ParentCollection)
                .WithMany(c => c.ChildCollections)
                .HasForeignKey(c => c.ParentCollectionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.CoverItem)
                .WithMany(i => i.CoveredCollections)
                .HasForeignKey(c => c.CoverItemId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            entity.HasIndex(c => c.ParentCollectionId);
            entity.HasIndex(c => new { c.ParentCollectionId, c.Name }).IsUnique(false);
            entity.HasQueryFilter(c => c.DeletedAt == null);
        });

        modelBuilder.Entity<ItemEntity>(entity =>
        {
            entity.ToTable(ItemTable.TABLE_NAME).HasKey(i => i.Id);
            entity.Property(i => i.Name).IsRequired().HasMaxLength(255);
            entity.Property(i => i.CreatedAt).IsRequired();
            entity.Property(i => i.UpdatedAt).IsRequired();

            // Collection relationship
            entity.HasOne(i => i.Collection)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CollectionId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(i => i.CollectionId);
            entity.HasQueryFilter(i => i.DeletedAt == null);
        });

        modelBuilder.Entity<CollectionFile>(entity =>
        {
            entity.ToTable(FileTable.TABLE_NAME).HasKey(f => f.Id);
            entity.Property(f => f.Name).IsRequired();
            entity.Property(f => f.Extension).IsRequired().HasMaxLength(20);
            entity.Property(f => f.MimeType).IsRequired().HasMaxLength(100);
            entity.Property(f => f.Bucket);
            entity.Property(f => f.StorageKey).IsRequired().HasMaxLength(1000);
            entity.Property(f => f.Size).IsRequired();
            entity.Property(f => f.ItemId);
            entity.Property(f => f.CreatedAt).IsRequired();
            entity.Property(f => f.UpdatedAt).IsRequired();
            entity.HasQueryFilter(f => f.DeletedAt == null);
            
            // One-to-one File <-> Item
            entity.HasOne(f => f.Item)
                .WithOne(i => i.CollectionFile)
                .HasForeignKey<CollectionFile>(f => f.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Tags>(entity =>
        {
            entity.ToTable(TagTable.TABLE_NAME).HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
            entity.Property(t => t.CreatedAt).IsRequired();
            entity.Property(t => t.UpdatedAt).IsRequired();
            entity.HasIndex(t => t.Name).IsUnique();
            entity.HasQueryFilter(t => t.DeletedAt == null);
        });

        modelBuilder.Entity<ItemTags>(entity =>
        {
            entity.ToTable(ItemTagTable.TABLE_NAME).HasKey(it => new { it.ItemId, it.TagId });

            entity.HasOne(it => it.Item)
                .WithMany(i => i.ItemTags)
                .HasForeignKey(it => it.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(it => it.Tag)
                .WithMany(t => t.ItemTags)
                .HasForeignKey(it => it.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

public sealed class WriteDbContext : BaseDbContext<WriteDbContext>
{
    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options) { }
}

public sealed class ReadDbContext : BaseDbContext<ReadDbContext>
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options) { }
}

public sealed class MigrateDbContext : BaseDbContext<MigrateDbContext>
{
    public MigrateDbContext(DbContextOptions<MigrateDbContext> options) : base(options) { }
}

// public class MigrateContextFactory : IDesignTimeDbContextFactory<MigrateDbContext>
// {
//     public MigrateDbContext CreateDbContext(string[] args)
//     {
//         // BUG: Hardcoding of the strings is working in the connection string. But loading from configuration is not.
//         // seems the directory is not right
//         IConfigurationRoot configuration = new ConfigurationBuilder()
//             .SetBasePath(Directory.GetCurrentDirectory())
//             .AddJsonFile("appsettings.json", optional: true)
//             .AddEnvironmentVariables()
//             .Build();
//
//         DbContextOptionsBuilder<MigrateDbContext> optionsBuilder = new DbContextOptionsBuilder<MigrateDbContext>();
//
//         // 2. Extract your migration-specific credentials
//         // You can hardcode this temporarily to test, or pull from config:
//         var user = configuration["Postgres:Migrate:Username"];
//         var pass = configuration["Postgres:Migrate:Password"];
//         var host = configuration["Postgres:Migrate:Host"];
//         var db = configuration["Postgres:Migrate:Database"];
//         var port = configuration["Postgres:Migrate:Port"];
//
//         string connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={pass}";
//
//         optionsBuilder.UseNpgsql(connectionString);
//
//         return new MigrateDbContext(optionsBuilder.Options);
//     }
// }