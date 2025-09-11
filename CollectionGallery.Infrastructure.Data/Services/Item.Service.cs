using System.Text.Json;
using CollectionGallery.Domain.Models.Controllers;
using CollectionGallery.Domain.Models.Entities;
using CollectionGallery.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CollectionGallery.InfraStructure.Data.Services;

public class ItemService
{
    private readonly CollectionGalleryContext _context;
    private readonly ModelService _modelService;
    private readonly CollectionService _collectionService;
    private readonly TagService _tagService;
    private readonly PlatformService _platformService;
    private readonly ILogger<ItemService> _logger;
    private readonly DbSet<Item> _itemContext;

    public ItemService(CollectionGalleryContext context, ModelService service, CollectionService collectionService, ILogger<ItemService> logger, TagService tagService, PlatformService platformService)
    {
        _context = context;
        _modelService = service;
        _collectionService = collectionService;
        _logger = logger;
        _tagService = tagService;
        _platformService = platformService;
        _itemContext = context.Items;
    }

    public async Task<MethodStatus> InsertItemAsync(FileUploadResultObject data)
    {
        DatabaseFacade database = _context.Database;
        try
        {
            using (await database.BeginTransactionAsync())
            {
                DateTime dateTime = DateTime.UtcNow;
                // Model model = await _modelService.InsertAsync(new Model { Name = data.Model, CreatedAt = dateTime, UpdatedAt = dateTime });
                Item? item = await SearchByName(data.FileName);

                if (item is not null)
                {
                    _logger.LogWarning("File ({0}) already exists in the DB. Skipping the insertion. Trace Id: {1}", data.FileName, data.TraceId);
                    return MethodStatus.SUCCESS;
                }

                Item newItem = new Item
                {
                    CreatedAt = dateTime,
                    Extension = data.Extension,
                    ModelId = data.ModelId,
                    Name = data.FileName,
                    ParentCollectionId = data.CollectionId == 0 ? null : data.CollectionId,
                    UpdatedAt = dateTime,
                };
                
                await _context.Items.AddAsync(newItem);
                await _context.SaveChangesAsync();
                await _tagService.AddItemTagsAsync(newItem.Id, data.Tags);
                await _platformService.AddPlatformTagsAsync(newItem.Id, data.Platforms);

                await database.CommitTransactionAsync();
                _logger.LogInformation("File ({0}) was insert in database succcessfully. Trace ID: {1}", data.FileName, data.TraceId);
                return MethodStatus.SUCCESS;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong when inserting file ({0}). Requested Object ({1}). Exception: {2}, TraceId: {3}", data.FileName, JsonSerializer.Serialize(data), e.Message, data.TraceId);
            await database.RollbackTransactionAsync();
            return MethodStatus.FAILURE;
        }
    }

    private async Task<Item?> SearchByName(string fileName)
    {
        Item? item = await _context.Items.FirstOrDefaultAsync(i => i.Name.ToLower() == fileName.ToLower());
        return item;
    }

    public async Task<List<ItemList>> ListAsync()
    {
        string storageServer = Environment.GetEnvironmentVariable("STORAGE_SERVER")!;
        List<ItemList> list = await _itemContext.Select(i => new ItemList { id = i.Id, Url = $"{storageServer}/{i.Name}" }).ToListAsync();
        return list;
    }
}