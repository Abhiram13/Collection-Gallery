using System.Data;
using System.Data.Common;
using System.Text.Json;
using CollectionGallery.Domain.Models.Controllers;
using CollectionGallery.Domain.Models.Entities;
using CollectionGallery.Domain.Models.Enums;
using CollectionGallery.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace CollectionGallery.InfraStructure.Data.Services;

public class ItemService : BaseService
{
    private readonly CollectionService _collectionService;
    private readonly TagService _tagService;
    private readonly ILogger<ItemService> _logger;

    public ItemService(
        CollectionGalleryWriteContext writeContext,
        CollectionGalleryReadContext readContext,
        CollectionService collectionService, 
        ILogger<ItemService> logger, 
        TagService tagService
    ) : base (writeContext, readContext)
    {
        _collectionService = collectionService;
        _logger = logger;
        _tagService = tagService;
    }

    public async Task<MethodStatus> InsertItemAsync(FileUploadResultObject data)
    {
        DatabaseFacade database = _writeContext.Database;
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

                await _writeContext.Items.AddAsync(newItem);
                await _writeContext.SaveChangesAsync();
                await _tagService.AddItemTagsAsync(newItem.Id, data.Tags);

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
        Item? item = await _readContext.Items.FirstOrDefaultAsync(i => i.Name.ToLower() == fileName.ToLower());
        return item;
    }

    public async Task<List<ItemList>> ListAsync()
    {
        string storageServer = Environment.GetEnvironmentVariable("STORAGE_SERVER")!;
        List<ItemList> list = await _readContext.Items.Select(i => new ItemList { Id = i.Id, Url = $"{storageServer}/{i.Name}" }).ToListAsync();
        List<ItemList> repeated = list.SelectMany(item => Enumerable.Repeat(item, 15)).ToList();
        return repeated;
    }

    public async Task<ItemDetails> ItemByIdAsync(int id)
    {
        const string QUERY = @"
            SELECT 
                i.id AS item_id, 
                i.name AS item_name, 
                JSON_AGG(JSONB_BUILD_OBJECT('id', m.id, 'name', m.name)) AS model_name, 
                JSON_AGG(JSONB_BUILD_OBJECT('id', t.id, 'name', t.name)) AS tags
            FROM items i
            LEFT JOIN models m ON m.id = i.model_id
            LEFT JOIN itemtags it ON it.item_id = i.id
            LEFT JOIN tags t ON t.id = it.tag_id
            WHERE i.id = @ItemId
            GROUP BY i.id, i.name, m.name
        ";

        _readContext.Database.OpenConnection();
        DbConnection connection = _readContext.Database.GetDbConnection();
        ItemDetails itemDetails = new ItemDetails();
        using (DbCommand command = connection.CreateCommand())
        {
            command.CommandText = QUERY;
            NpgsqlParameter parameter = new NpgsqlParameter
            {
                ParameterName = "@ItemId",
                Value = id,
                Direction = System.Data.ParameterDirection.Input,
                DbType = System.Data.DbType.Int32,
            };

            command.Parameters.Add(parameter);
            using (DbDataReader? reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    itemDetails.Id = reader.GetInt32(0);
                    itemDetails.Name = reader.GetString(1);
                    itemDetails.Models = JsonSerializer.Deserialize<List<ItemDetails.SubDetails>>(reader.GetString(2));
                    itemDetails.Tags = JsonSerializer.Deserialize<List<ItemDetails.SubDetails>>(reader.GetString(3));
                }
            }
        }

        return itemDetails;
    }

    public async Task ValidateUploadAsync(ValidateUploadDto payload)
    {
        if (payload.CollectionId is not null && payload.CollectionId > 0)
        {
            int collectionId = (int)payload.CollectionId;
            bool isCollectionExist = await _collectionService.IsCollectionExist(collectionId);

            if (!isCollectionExist) throw new CollectionIdNotFoundException($"Given Collection ID ({collectionId}) is invalid or not exists");
        }

        if (payload.Tags is not null && payload.Tags.Length > 0)
        {
            foreach (int tag in payload.Tags)
            {
                if (tag > 0)
                {
                    bool isTagExist = await _tagService.IsTagExistAsync(tag);

                    if (!isTagExist) throw new TagIdNotFoundException($"Given Tag ID ({tag}) is invalid or not exists");
                }
            }
        }
    }
    
    public async Task UploadSuccessMetaData(UploadSuccessDto payload)
    {
        using (IDbContextTransaction? transaction = await _readContext.Database.BeginTransactionAsync())
        {
            try
            {
                DateTime date = DateTime.UtcNow;
                Item item = new Item();
                item.CreatedAt = date;
                item.UpdatedAt = date;
                item.Extension = payload.Extension;
                item.Name = payload.FileName;
                item.ParentCollectionId = payload.CollectionId;
                item.Size = FileSize.Original;

                await _writeContext.AddAsync(item);
                await _writeContext.SaveChangesAsync();

                if (payload.Tags is not null && payload.Tags.Length > 0)
                {
                    await _tagService.AddItemTagsAsync(item.Id, payload.Tags.ToList());
                }

                await transaction.CommitAsync();
            }
            catch(Exception e)
            {
                _logger.LogError(exception: e, message: e.Message);
                await transaction.RollbackAsync();
            }
        }
    }
}