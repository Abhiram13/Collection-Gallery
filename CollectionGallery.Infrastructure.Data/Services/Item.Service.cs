using CollectionGallery.InfraStructure.Data.Entities;
using CollectionGallery.InfraStructure.Data.Enums;
using CollectionGallery.InfraStructure.Data.HttpClients;
using CollectionGallery.InfraStructure.Data.Item.Models;
using CollectionGallery.InfraStructure.Data.Repository;
using CollectionGallery.Shared.Models;

namespace CollectionGallery.InfraStructure.Data.Services;

public class ItemService
{
    private readonly ItemRepository _itemRepository;
    private readonly CloudStorageHttpClient _cloudStorageHttpClient;

    public ItemService(ItemRepository itemRepository, CloudStorageHttpClient cloudStorageHttpClient)
    {
        _itemRepository = itemRepository;
        _cloudStorageHttpClient = cloudStorageHttpClient;
    }

    // TODO: There is a chance an exception occurs. Catch and re-throw error and rollback changes
    public async Task<SignedUrlResponseDto?> InsertOneItemAsync(ItemInsertDto item)
    {
        ItemEntity itemEntity = ItemEntity.Create(item.Name, item.CollectionId);
        ItemEntity insertedItem = await _itemRepository.InsertOneItemAsync(itemEntity);
        SignedUrlResponseDto? response = await _cloudStorageHttpClient.GetSignedUrlAsync(new GetSignedUrlDto { FileName = item.FileName!, ItemId = insertedItem.Id });

        return response;
    }
}