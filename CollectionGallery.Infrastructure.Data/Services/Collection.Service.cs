using Microsoft.EntityFrameworkCore;
using CollectionGallery.Domain.Models.Entities;
using CollectionGallery.Domain.Models.Enums;
using CollectionGallery.Domain.Models.Controllers;
using System.Data.Common;
using Npgsql;
using System.Text.Json;

namespace CollectionGallery.InfraStructure.Data.Services;

public class CollectionService
{
    private readonly CollectionGalleryContext _context;
    private readonly DbSet<Collection> _collectionDataSet;
    private readonly ILogger<CollectionService> _logger;
    private DateTime _dateTime;

    public CollectionService(CollectionGalleryContext context, ILogger<CollectionService> logger)
    {
        _context = context;
        _collectionDataSet = _context.Collections;
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

        await _collectionDataSet.AddAsync(collection);
        await _context.SaveChangesAsync();
        return collection;
    }

    private async Task<Collection?> GetCollectionByName(string collectionName)
    {
        Collection? collection = await _collectionDataSet.FirstOrDefaultAsync(c => c.Name.ToLower() == collectionName.ToLower());
        return collection;
    }

    public async Task<List<ParentCollections>> ListOfParentCollections()
    {
        List<ParentCollections> parentCollections = await _collectionDataSet
            .Where(c => c.ParentCollectionId == null)
            .Select(c => new ParentCollections { CollectionPic = c.CollectionPic, Id = c.Id, Name = c.Name })
            .ToListAsync();

        return parentCollections;
    }

    public async Task<CollectionDetailsById> CollectionsById(int id)
    {
        const string QUERY = @"
            SELECT 
                parent.id, parent.name, parent.created_at, parent.updated_at, parent.collection_pic,
                COALESCE(
                    (
                        SELECT JSON_AGG(JSON_BUILD_OBJECT('id', item.id, 'name', item.name))
                        FROM items item
                        WHERE item.parent_collection_id = parent.id
                    ), '[]'::json
                ) AS collectionItems,
                COALESCE(
                    (
                        SELECT JSON_AGG(JSON_BUILD_OBJECT('id', platform.id, 'name', platform.name))
                        FROM platforms platform
                        WHERE platform.id IN (
                            SELECT ip.platform_id 
                            FROM itemplatforms ip
                            JOIN items i ON i.id = ip.item_id
                            WHERE i.parent_collection_id = parent.id
                        )
                    ), '[]'::json
                ) AS collectionPlatforms,
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

        _context.Database.OpenConnection();
        DbConnection connection = _context.Database.GetDbConnection();
        CollectionDetailsById details = new CollectionDetailsById();
        JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        string? STORAGE_HOST = Environment.GetEnvironmentVariable("STORAGE_SERVER");
        using (DbCommand command = connection.CreateCommand())
        {
            command.CommandText = QUERY;
            NpgsqlParameter parameter = new NpgsqlParameter
            {
                ParameterName = "@ParentId",
                Value = id,
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
                    details.Items = JsonSerializer.Deserialize<List<CollectionDetailsById.CollectionItems>>(reader.GetString(5), options) ?? new();
                    details.Platforms = JsonSerializer.Deserialize<List<CollectionDetailsById.CollectionPlatforms>>(reader.GetString(6), options) ?? new();
                    details.Collections = JsonSerializer.Deserialize<List<CollectionDetailsById.ChildCollection>>(reader.GetString(7), options) ?? new();
                }
            }

            foreach (CollectionDetailsById.CollectionItems detail in details.Items)
            {
                detail.Name = $"{STORAGE_HOST}/{detail.Name}";
            }
        }

        return details;
    }

    private async Task<bool> IsCollectionExist(int collectionId)
    {
        int collectionCount = await _collectionDataSet.CountAsync(c => c.Id == collectionId);
        return collectionCount > 0;
    }

    public async Task<UpdateFieldResult> UpdateByIdAsync(int collectionId, Collection body)
    {
        Collection? existingCollection = await _collectionDataSet.FindAsync(collectionId);

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

        await _context.SaveChangesAsync();
        return UpdateFieldResult.Success;
    }
}