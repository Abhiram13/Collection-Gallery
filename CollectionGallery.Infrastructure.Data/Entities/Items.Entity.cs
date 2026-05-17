using System.ComponentModel.DataAnnotations.Schema;
using CollectionGallery.InfraStructure.Data.Enums;
using CollectionGallery.InfraStructure.Data.Constants;

using ItemTable = CollectionGallery.InfraStructure.Data.Constants.DbTableNames.Item;
using ItemTagsTable = CollectionGallery.InfraStructure.Data.Constants.DbTableNames.ItemTag;

namespace CollectionGallery.InfraStructure.Data.Entities;

[Table(ItemTable.TABLE_NAME)]
public class Item : BaseEntity
{
    [Column(ItemTable.NAME)]
    public string Name { get; set; } = string.Empty;

    [Column(ItemTable.FILE_ID)]
    public int FileId { get; set; }

    [Column(ItemTable.COLLECTION_ID)]
    public int? CollectionId { get; set; } = null;
    
    public CollectionEntity? Collection { get; set; } = default!;
    public CollectionFile CollectionFile { get; set; } = default!;
    public ICollection<ItemTags> ItemTags { get; set; } = new List<ItemTags>();
    public ICollection<CollectionEntity> CoveredCollections { get; set; } = new List<CollectionEntity>();
}

[Table(ItemTagsTable.TABLE_NAME)]
public class ItemTags
{
    [Column(ItemTagsTable.ITEM_ID)]
    public int ItemId { get; set; }

    [Column(ItemTagsTable.TAG_ID)]
    public int TagId { get; set; }
    
    public ItemEntity Item { get; set; } = default!;
    public Tags Tag { get; set; } = default!;
}