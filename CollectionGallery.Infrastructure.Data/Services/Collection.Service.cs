using Microsoft.EntityFrameworkCore;
using CollectionGallery.Domain.Models.Entities;
using CollectionGallery.Domain.Models.Enums;
using CollectionGallery.Domain.Models.Controllers;
using System.Data.Common;
using Npgsql;
using System.Text.Json;

namespace CollectionGallery.InfraStructure.Data.Services;

public class CollectionService : BaseService
{
    private readonly ILogger<CollectionService> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    public CollectionService(CollectionGalleryWriteContext writeContext, CollectionGalleryReadContext readContext, ILogger<CollectionService> logger) : base(writeContext, readContext)
    {
        _logger = logger;
    }

    // TODO: Check for valid Parent Folder ID
    // TODO: Add Validations
    public async Task<Collection> InsertAsync(Collection collection)
    {
        Collection? existingCollection = await GetCollectionByName(collection.Name);

        if (existingCollection is not null)
        {
            _logger.LogWarning("Collection ({0}) already exists. Hence skipping at inserting in DB", collection.Name);
            return existingCollection;
        }

        await _writeContext.Collections.AddAsync(collection);
        await _writeContext.SaveChangesAsync();
        return collection;
    }

    private async Task<Collection?> GetCollectionByName(string collectionName)
    {
        Collection? collection = await _readContext.Collections.FirstOrDefaultAsync(c => c.Name.ToLower() == collectionName.ToLower());
        return collection;
    }

    public async Task<List<ParentCollections>> ListOfParentCollections()
    {
        List<ParentCollections> parentCollections = await _readContext.Collections
            .Where(c => c.ParentCollectionId == null)
            .Select(c => new ParentCollections { CollectionPic = "https://static.vecteezy.com/vite/assets/photo-masthead-375-BoK_p8LG.webp", Id = c.Id, Name = c.Name })
            .ToListAsync();

        List<ParentCollections> repeated = parentCollections.SelectMany(col => Enumerable.Repeat(col, 15)).ToList();

        return repeated;
    }

    public async Task<CollectionDetailsById> CollectionsById(int collectionId)
    {
        const string QUERY = @"
            SELECT 
                parent.id, parent.name, parent.created_at, parent.updated_at, parent.collection_pic,
                COALESCE(
                    (
                        SELECT JSON_AGG(JSON_BUILD_OBJECT('id', child.id, 'name', child.name, 'collectionPic', child.collection_pic))
                        FROM collections child
                        WHERE child.parent_collection_id = parent.id
                    ), '[]'::json
                ) AS childCollection
            FROM collections parent
            WHERE parent.id = @ParentId
            GROUP BY parent.id;
        ";

        _readContext.Database.OpenConnection();
        DbConnection connection = _readContext.Database.GetDbConnection();
        CollectionDetailsById details = new CollectionDetailsById();
        string? STORAGE_HOST = Environment.GetEnvironmentVariable("STORAGE_SERVER");
        using (DbCommand command = connection.CreateCommand())
        {
            command.CommandText = QUERY;
            NpgsqlParameter parameter = new NpgsqlParameter
            {
                ParameterName = "@ParentId",
                Value = collectionId,
                Direction = System.Data.ParameterDirection.Input,
                DbType = System.Data.DbType.Int32,
            };

            command.Parameters.Add(parameter);
            using (DbDataReader? reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    details.Id = reader.GetInt32(0);
                    details.Name = reader.GetString(1);
                    details.CreatedAt = reader.GetDateTime(2);
                    details.UpdatedAt = reader.GetDateTime(3);
                    details.CollectionPic = reader.IsDBNull(4) ? "" : reader.GetString(4);
                    details.Collections = JsonSerializer.Deserialize<List<CollectionDetailsById.ChildCollection>>(reader.GetString(5), _jsonOptions) ?? new();
                }
            }
        }

        return details;
    }

    public async Task<List<ItemsByCollectionId>> GetItemsByCollectionIdAsync(int collectionId)
    {
        const string QUERY = @"
            SELECT item.id, item.name
            FROM items item
            WHERE item.parent_collection_id = @CollectionId
        ";

        _readContext.Database.OpenConnection();
        DbConnection connection = _readContext.Database.GetDbConnection();
        List<ItemsByCollectionId> itemsByCollectionId = new List<ItemsByCollectionId>();
        using (DbCommand command = connection.CreateCommand())
        {
            command.CommandText = QUERY;
            NpgsqlParameter parameter = new NpgsqlParameter
            {
                ParameterName = "@CollectionId",
                Value = collectionId,
                Direction = System.Data.ParameterDirection.Input,
                DbType = System.Data.DbType.Int32
            };

            command.Parameters.Add(parameter);
            using (DbDataReader? reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    ItemsByCollectionId item = new ItemsByCollectionId
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };

                    itemsByCollectionId.Add(item);
                }
            }
        }

        return itemsByCollectionId;    
    }

    public async Task<bool> IsCollectionExist(int collectionId)
    {
        int collectionCount = await _readContext.Collections.CountAsync(c => c.Id == collectionId);
        return collectionCount > 0;
    }

    public async Task<UpdateFieldResult> UpdateByIdAsync(int collectionId, Collection body)
    {
        Collection? existingCollection = await _readContext.Collections.FindAsync(collectionId);

        if (body.ParentCollectionId is not null && body.ParentCollectionId != 0)
        {
            bool isParentCollectionExist = await IsCollectionExist(body.ParentCollectionId ?? 0);

            if (isParentCollectionExist == false) return UpdateFieldResult.ParentNotFound;
        }

        if (existingCollection is null)
        {
            return UpdateFieldResult.NotFound;
        }

        if (!string.IsNullOrEmpty(body.Name)) existingCollection.Name = body.Name;
        if (body.CollectionPic is not null) existingCollection.CollectionPic = body.CollectionPic;
        if (body.ParentCollectionId is not null) existingCollection.ParentCollectionId = body.ParentCollectionId;
        existingCollection.UpdatedAt = DateTime.UtcNow;

        await _writeContext.SaveChangesAsync();
        return UpdateFieldResult.Success;
    }
}