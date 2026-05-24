namespace CollectionGallery.InfraStructure.Data.Repository;

public abstract class BaseRepository
{
    protected readonly WriteDbContext _writeDbContext;
    protected readonly ReadDbContext _readDbContext;

    protected BaseRepository(WriteDbContext writeDbContext, ReadDbContext readDbContext)
    {
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
    }
}