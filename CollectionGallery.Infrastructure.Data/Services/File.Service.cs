using CollectionGallery.Domain.Models.Controllers;
using CollectionGallery.Domain.Models.Entities;
using CollectionGallery.Domain.Models.Enums;
using CollectionGallery.Shared;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CollectionGallery.InfraStructure.Data.Services;

public class FileService
{
    private readonly CollectionGalleryContext _context;
    private readonly ModelService _modelService;
    private readonly CollectionService _collectionService;

    public FileService(CollectionGalleryContext context, ModelService service, CollectionService collectionService)
    {
        _context = context;
        _modelService = service;
        _collectionService = collectionService;
    }

    public async Task<MethodStatus> InsertFileAsync(FileUploadResultObject data)
    {
        DatabaseFacade database = _context.Database;
        try
        {
            using (await database.BeginTransactionAsync())
            {
                DateTime dateTime = DateTime.UtcNow;
                Model model = await _modelService.InsertAsync(new Model { Name = data.Model, CreatedAt = dateTime, UpdatedAt = dateTime }, traceId: data.TraceId);
                await _context.Items.AddAsync(new Item
                {
                    CreatedAt = dateTime,
                    Extension = data.Extension,
                    ModelId = model.Id,
                    Name = data.FileName,
                    ParentCollectionId = data.CollectionId == 0 ? null : data.CollectionId,
                    UpdatedAt = dateTime,
                });
                await _context.SaveChangesAsync();
                await database.CommitTransactionAsync();
                return MethodStatus.SUCCESS;
            }
        }
        catch (Exception e)
        {
            await database.RollbackTransactionAsync();
            Logger.LogError(e, e.Message);
            return MethodStatus.FAILURE;
        }        
    }
}