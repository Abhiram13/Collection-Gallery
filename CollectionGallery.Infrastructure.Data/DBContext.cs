using Microsoft.EntityFrameworkCore;
using CollectionGallery.InfraStructure.Data.Entities;

namespace CollectionGallery.InfraStructure.Data
{
    public abstract class BaseDbContext<TContext> : DbContext where TContext : DbContext
    {
        public BaseDbContext(DbContextOptions<TContext> options) : base(options) { }
        
        public DbSet<Model> Models { get; init; }
        public DbSet<ItemEntity> Items { get; init; }
        public DbSet<Tags> Tags { get; init; }
        public DbSet<CollectionEntity> Collections { get; init; }
        public DbSet<ItemTags> ItemTags { get; init; }
    }

    public sealed class WriteDBContext : BaseDbContext<WriteDBContext>
    {
        public WriteDBContext(DbContextOptions<WriteDBContext> options) : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Item tags
            modelBuilder.Entity<ItemTags>().HasKey(it => new { it.ItemId, it.TagId });
            modelBuilder.Entity<ItemTags>().HasOne(it => it.Item).WithMany(i => i.ItemTags).HasForeignKey(it => it.ItemId);
            modelBuilder.Entity<ItemTags>().HasOne(it => it.Tag).WithMany(t => t.ItemTags).HasForeignKey(it => it.TagId);
            // Item tags
        }
    }

    public sealed class ReadDbContext : BaseDbContext<ReadDbContext>
    {
        public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options) { }
    }
    
    public sealed class MigrateDbContext : BaseDbContext<MigrateDbContext>
    {
        public MigrateDbContext(DbContextOptions<MigrateDbContext> options) : base(options) { }
    }
}