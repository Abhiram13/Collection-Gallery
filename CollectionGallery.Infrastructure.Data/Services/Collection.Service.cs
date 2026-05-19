using System.Data.Common;
using System.Text.Json;
using CollectionGallery.InfraStructure.Data.Collection.Models;
using CollectionGallery.InfraStructure.Data.Entities;
using CollectionGallery.InfraStructure.Data.Enums;
using CollectionGallery.InfraStructure.Data.Repository;
using CollectionGallery.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CollectionGallery.InfraStructure.Data.Services;

public class CollectionService
{
    private readonly ILogger<CollectionService> _logger;
    private readonly CollectionRepository _repository;

    public CollectionService(ILogger<CollectionService> logger, CollectionRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    // TODO: Check for valid Parent Folder ID
    // TODO: Add Validations
    public async Task InsertAsync(InsertCollectionDto payload)
    {
        CollectionEntity? collectionEntity = await _repository.GetCollectionByNameAsync(payload.Name);

        if (collectionEntity is not null)
        {
            throw new InvalidPayloadException($"Collection with name ({payload.Name}) already exists");
        }
        
        collectionEntity = CollectionEntity.Create(payload.Name, payload.ParentCollectionId, payload.CoverItemId);
        
        await _repository.InsertOneAsync(collectionEntity);
    }

    // private async Task<CollectionEntity?> GetCollectionByName(string collectionName)
    // {
    //     CollectionEntity? collection = await _collectionDataSet.FirstOrDefaultAsync(c => c.Name.ToLower() == collectionName.ToLower());
    //     return collection;
    // }
    //
    // public async Task<List<ParentCollections>> ListOfParentCollections()
    // {
    //     // List<ParentCollections> parentCollections = await _collectionDataSet
    //     //     .Where(c => c.ParentCollectionId == null)
    //     //     .Select(c => new ParentCollections { CollectionPic = c.CollectionPic, Id = c.Id, Name = c.Name })
    //     //     .ToListAsync();
    //
    //     // return parentCollections;
    //
    //     return new List<ParentCollections>();
    // }
    //
    // public async Task<CollectionDetailsById> CollectionsById(int id)
    // {
    //     const string QUERY = @"
    //         SELECT 
    //             parent.id, parent.name, parent.created_at, parent.updated_at, parent.collection_pic,
    //             COALESCE(
    //                 (
    //                     SELECT JSON_AGG(JSON_BUILD_OBJECT('id', item.id, 'name', item.name))
    //                     FROM items item
    //                     WHERE item.parent_collection_id = parent.id
    //                 ), '[]'::JSON
    //             ) AS collectionItems,
    //             COALESCE(
    //                 (
    //                     SELECT JSON_AGG(JSON_BUILD_OBJECT('id', platform.id, 'name', platform.name))
    //                     FROM platforms platform
    //                     WHERE platform.id IN (
    //                         SELECT ip.platform_id 
    //                         FROM itemplatforms ip
    //                         JOIN items i ON i.id = ip.item_id
    //                         WHERE i.parent_collection_id = parent.id
    //                     )
    //                 ), '[]'::json
    //             ) AS collectionPlatforms,
    //             COALESCE(
    //                 (
    //                     SELECT JSON_AGG(JSON_BUILD_OBJECT('id', child.id, 'name', child.name, 'collectionPic', child.collection_pic))
    //                     FROM collections child
    //                     WHERE child.parent_collection_id = parent.id
    //                 ), '[]'::json
    //             ) AS childCollection
    //             FROM collections parent
    //             WHERE parent.id = @ParentId
    //             GROUP BY parent.id;
    //     ";
    //
    //     _context.Database.OpenConnection();
    //     DbConnection connection = _context.Database.GetDbConnection();
    //     CollectionDetailsById details = new CollectionDetailsById();
    //     JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    //     string? STORAGE_HOST = Environment.GetEnvironmentVariable("STORAGE_SERVER");
    //     using (DbCommand command = connection.CreateCommand())
    //     {
    //         command.CommandText = QUERY;
    //         NpgsqlParameter parameter = new NpgsqlParameter
    //         {
    //             ParameterName = "@ParentId",
    //             Value = id,
    //             Direction = System.Data.ParameterDirection.Input,
    //             DbType = System.Data.DbType.Int32,
    //         };
    //
    //         command.Parameters.Add(parameter);
    //         using (DbDataReader? reader = await command.ExecuteReaderAsync())
    //         {
    //             while (await reader.ReadAsync())
    //             {
    //                 details.Id = reader.GetInt32(0);
    //                 details.Name = reader.GetString(1);
    //                 details.CreatedAt = reader.GetDateTime(2);
    //                 details.UpdatedAt = reader.GetDateTime(3);
    //                 details.CollectionPic = reader.IsDBNull(4) ? "" : reader.GetString(4);
    //                 details.Items = JsonSerializer.Deserialize<List<CollectionDetailsById.CollectionItems>>(reader.GetString(5), options) ?? new();
    //                 details.Platforms = JsonSerializer.Deserialize<List<CollectionDetailsById.CollectionPlatforms>>(reader.GetString(6), options) ?? new();
    //                 details.Collections = JsonSerializer.Deserialize<List<CollectionDetailsById.ChildCollection>>(reader.GetString(7), options) ?? new();
    //             }
    //         }
    //
    //         foreach (CollectionDetailsById.CollectionItems detail in details.Items)
    //         {
    //             detail.Name = $"{STORAGE_HOST}/{detail.Name}";
    //         }
    //     }
    //
    //     return details;
    // }
    //
    // private async Task<bool> IsCollectionExist(int collectionId)
    // {
    //     // int collectionCount = await _collectionDataSet.CountAsync(c => c.Id == collectionId);
    //     // return collectionCount > 0;
    //
    //     return true;
    // }
    //
    // public async Task<UpdateFieldResult> UpdateByIdAsync(int collectionId, CollectionEntity body)
    // {
    //     CollectionEntity? existingCollection = await _collectionDataSet.FindAsync(collectionId);
    //
    //     if (body.ParentCollectionId is not null && body.ParentCollectionId != 0)
    //     {
    //         bool isParentCollectionExist = await IsCollectionExist(body.ParentCollectionId ?? 0);
    //
    //         if (isParentCollectionExist == false) return UpdateFieldResult.ParentNotFound;
    //     }
    //
    //     if (existingCollection is null)
    //     {
    //         return UpdateFieldResult.NotFound;
    //     }
    //
    //     // if (!string.IsNullOrEmpty(body.Name)) existingCollection.Name = body.Name;
    //     // if (body.CollectionPic is not null) existingCollection.CollectionPic = body.CollectionPic;
    //     // if (body.ParentCollectionId is not null) existingCollection.ParentCollectionId = body.ParentCollectionId;
    //     existingCollection.UpdatedAt = DateTime.UtcNow;
    //
    //     await _context.SaveChangesAsync();
    //     return UpdateFieldResult.Success;
    // }
}