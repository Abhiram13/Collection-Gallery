using CollectionGallery.Domain.Models.Controllers;
using CollectionGallery.Domain.Models.Entities;
using CollectionGallery.Domain.Models.Enums;

namespace CollectionGallery.Domain.Interfaces;

/// <summary>
/// Defines DB operations for the <see cref="Collection"/> Entity
/// </summary>
public interface ICollectionRepository
{
    Task<Collection> InsertAsync(Collection collection);
    Task<Collection?> GetCollectionByNameAsync(string collectionName);
    Task<List<ParentCollections>> GetParentCollectionsAsync();
    Task<CollectionDetailsById> GetCollectionById(int collectionId);
    Task<UpdateFieldResult> UpdateByIdAsync(int collectionId, Collection collection);
}