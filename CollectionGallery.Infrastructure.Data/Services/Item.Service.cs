using System.Data.Common;
using System.Text.Json;
using CollectionGallery.InfraStructure.Data.Entities;
using CollectionGallery.InfraStructure.Data.Enums;
using CollectionGallery.InfraStructure.Data.Item.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql;

namespace CollectionGallery.InfraStructure.Data.Services;

public class ItemService
{
    private readonly WriteDbContext _context;
    private readonly CollectionService _collectionService;
    private readonly TagService _tagService;
    private readonly ILogger<ItemService> _logger;
    private readonly DbSet<ItemEntity> _itemContext;

    public ItemService(WriteDbContext context, CollectionService collectionService, ILogger<ItemService> logger, TagService tagService)
    {
        _context = context;
        _collectionService = collectionService;
        _logger = logger;
        _tagService = tagService;
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
                ItemEntity? item = await SearchByName(data.FileName);

                if (item is not null)
                {
                    _logger.LogWarning("File ({0}) already exists in the DB. Skipping the insertion. Trace Id: {1}", data.FileName, data.TraceId);
                    return MethodStatus.SUCCESS;
                }

                // ItemEntity newItem = new ItemEntity
                // {
                //     CreatedAt = dateTime,
                //     Extension = data.Extension,
                //     ModelId = data.ModelId,
                //     Name = data.FileName,
                //     ParentCollectionId = data.CollectionId == 0 ? null : data.CollectionId,
                //     UpdatedAt = dateTime,
                // };

                // await _context.Items.AddAsync(newItem);
                await _context.SaveChangesAsync();
                // await _tagService.AddItemTagsAsync(newItem.Id, data.Tags);

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

    private async Task<ItemEntity?> SearchByName(string fileName)
    {
        ItemEntity? item = await _context.Items.FirstOrDefaultAsync(i => i.Name.ToLower() == fileName.ToLower());
        return item;
    }

    public async Task<List<ItemList>> ListAsync()
    {
        string storageServer = Environment.GetEnvironmentVariable("STORAGE_SERVER")!;
        // List<ItemList> list = await _itemContext.Select(i => new ItemList { Id = i.Id, Url = $"{storageServer}/{i.Name}" }).ToListAsync();
        // List<ItemList> repeated = list.SelectMany(item => Enumerable.Repeat(item, 20)).ToList();
        // return repeated;

        return new List<ItemList>();
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

        _context.Database.OpenConnection();
        DbConnection connection = _context.Database.GetDbConnection();
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
}