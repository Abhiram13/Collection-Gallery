namespace CollectionGallery.InfraStructure.Data.Services;

public abstract class BaseService
{
    protected readonly CollectionGalleryWriteContext _writeContext;
    protected readonly CollectionGalleryReadContext _readContext;

    protected BaseService(CollectionGalleryWriteContext writeContext, CollectionGalleryReadContext readContext)
    {
        _writeContext = writeContext;
        _readContext = readContext;
    }
}