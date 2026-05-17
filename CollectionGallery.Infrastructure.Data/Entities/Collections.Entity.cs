using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using CollectionTable = CollectionGallery.InfraStructure.Data.Constants.DbTableNames.Collection;

namespace CollectionGallery.InfraStructure.Data.Entities;

[Table(CollectionTable.TABLE_NAME)]
public class Collection : BaseEntity
{
    [Column(CollectionTable.NAME)]
    public string Name { get; set; } = string.Empty;

    [Column(CollectionTable.PARENT_COLLECTION_ID)]
    public int? ParentCollectionId { get; set; } = null;

    [Column(CollectionTable.COVER_ITEM_ID)]
    public int? CoverItemId { get; set; } = null;
    
    public CollectionEntity? ParentCollection { get; set; }
    public ICollection<CollectionEntity> ChildCollections { get; set; } = new List<CollectionEntity>();
    public ItemEntity? CoverItem { get; set; }
    public ICollection<ItemEntity> Items { get; set; } = new List<ItemEntity>();
}