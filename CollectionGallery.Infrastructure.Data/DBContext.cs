using Microsoft.EntityFrameworkCore;
using CollectionGallery.Domain.Models.Entities;

namespace CollectionGallery.InfraStructure.Data
{
    public class CollectionGalleryContext : DbContext
    {
        public CollectionGalleryContext(DbContextOptions<CollectionGalleryContext> options) : base(options) { }
        public DbSet<Model> Models { get; init; }
        public DbSet<Item> Items { get; init; }
        public DbSet<Tags> Tags { get; init; }
        public DbSet<Platforms> Platforms { get; init; }
        public DbSet<Collection> Collections { get; init; }
        public DbSet<ItemTags> ItemTags { get; init; }
        public DbSet<ItemPlatforms> ItemPlatforms { get; init; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // base.OnModelCreating(modelBuilder);
            // FileTags
            modelBuilder.Entity<ItemTags>().HasKey(it => new { it.ItemId, it.TagId });
            modelBuilder.Entity<ItemTags>().HasOne(it => it.Item).WithMany(i => i.FileTags).HasForeignKey(it => it.ItemId);
            modelBuilder.Entity<ItemTags>().HasOne(it => it.Tag).WithMany(t => t.FileTags).HasForeignKey(it => it.TagId);
            // ImageTags

            // ImagePlatforms
            modelBuilder.Entity<ItemPlatforms>().HasKey(it => new { it.ItemId, it.PlatformId });
            modelBuilder.Entity<ItemPlatforms>().HasOne(it => it.Item).WithMany(i => i.FilePlatforms).HasForeignKey(it => it.ItemId);
            modelBuilder.Entity<ItemPlatforms>().HasOne(it => it.Platform).WithMany(t => t.FilePlatforms).HasForeignKey(it => it.PlatformId);
            // ImagePlatforms       
        }
    }
}