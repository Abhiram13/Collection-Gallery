using CollectionGallery.InfraStructure.Data.Entities;
using CollectionGallery.InfraStructure.Data.File.Models;
using CollectionGallery.InfraStructure.Data.Repository;

namespace CollectionGallery.InfraStructure.Data.Services;

public class FileService
{
    private readonly FileRepository _fileRepository;
    
    public FileService(FileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task InsertOneAsync(InsertCollectionFileDto payload)
    {
        CollectionFile file = CollectionFile.Create(
            name: payload.Name,
            extension: payload.Extension,
            size: payload.Size,
            mimeType: payload.MimeType,
            bucket: payload.Bucket,
            itemId: payload.ItemId
        );
        
        await _fileRepository.InsertOneAsync(file);
    }
}