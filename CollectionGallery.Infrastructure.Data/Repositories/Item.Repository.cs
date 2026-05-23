using CollectionGallery.InfraStructure.Data.Controllers;
using CollectionGallery.InfraStructure.Data.Entities;
using CollectionGallery.InfraStructure.Data.Item.Models;

namespace CollectionGallery.InfraStructure.Data.Repository;

public class ItemRepository
{
    private readonly WriteDbContext _writeDbContext;
    private readonly ReadDbContext _readDbContext;

    public ItemRepository(WriteDbContext writeDbContext, ReadDbContext readDbContext)
    {
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
    }

    public async Task<ItemEntity> InsertOneItemAsync(ItemEntity item)
    {
        await _writeDbContext.Items.AddAsync(item);
        await _writeDbContext.SaveChangesAsync();
        
        return item;
    }
}