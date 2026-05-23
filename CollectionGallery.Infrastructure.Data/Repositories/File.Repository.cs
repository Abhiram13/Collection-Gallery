using CollectionGallery.InfraStructure.Data.Entities;

namespace CollectionGallery.InfraStructure.Data.Repository;

public class FileRepository
{
    private readonly WriteDbContext _writeDbContext;
    private readonly ReadDbContext _readDbContext;
    
    public FileRepository(WriteDbContext writeDbContext, ReadDbContext readDbContext)
    {
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
    }

    public async Task InsertOneAsync(CollectionFile payload)
    {
        await _writeDbContext.CollectionFiles.AddAsync(payload);
        await _writeDbContext.SaveChangesAsync();
    }
}