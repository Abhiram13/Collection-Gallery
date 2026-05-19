using System.Data.Common;
using System.Text.Json;
using CollectionGallery.InfraStructure.Data.Collection.Models;
using CollectionGallery.InfraStructure.Data.Entities;
using CollectionGallery.InfraStructure.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CollectionGallery.InfraStructure.Data.Repository;

public class CollectionRepository
{
    private readonly WriteDbContext _writeDbContext;
    private readonly ReadDbContext _readDbContext;

    public CollectionRepository(WriteDbContext context, ReadDbContext readDbContext)
    {
        _writeDbContext = context;
        _readDbContext = readDbContext;
    }
    
    public async Task<CollectionDetailsById> GetCollectionById(int collectionId)
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

        _readDbContext.Database.OpenConnection();
        DbConnection connection = _readDbContext.Database.GetDbConnection();
        CollectionDetailsById details = new CollectionDetailsById();
        JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
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

    public async Task<IReadOnlyList<ParentCollection>> GetAllParentCollectionsAsync()
    {
        IReadOnlyList<ParentCollection> list = await _readDbContext.Collections
            .Select(c => new ParentCollection 
            {
                Id = c.Id,
                Name = c.Name, 
            }).ToListAsync();

        return list;
    }

    public async Task InsertOneAsync(CollectionEntity collection)
    {
        await _writeDbContext.Collections.AddAsync(collection);
        await _writeDbContext.SaveChangesAsync();
    }

    public async Task<CollectionEntity?> GetCollectionByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
    
    // TODO: Update projection based on scenarios. Pulling everything does not seems right.
    public async Task<CollectionEntity?> GetCollectionByNameAsync(string collectionName)
    {
        CollectionEntity? collection = await _readDbContext.Collections.Where(c => c.Name == collectionName).FirstOrDefaultAsync();
        
        return collection;
    }

    public Task<UpdateFieldResult> UpdateByIdAsync(int collectionId, CollectionEntity collection)
    {
        throw new NotImplementedException();
    }
}