using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using CollectionTable = CollectionGallery.InfraStructure.Data.Constants.DbTableNames.Collection;

namespace CollectionGallery.InfraStructure.Data.Entities;

[Table(CollectionTable.TABLE_NAME)]
public class CollectionEntity : BaseEntity
{
    [Column(CollectionTable.NAME)]
    public string Name { get; private set; } = string.Empty;

    [Column(CollectionTable.PARENT_COLLECTION_ID)]
    public int? ParentCollectionId { get; private set; } = null;

    [Column(CollectionTable.COVER_ITEM_ID)]
    public int? CoverItemId { get; private set; } = null;
    
    public CollectionEntity? ParentCollection { get; private set; }
    public ICollection<CollectionEntity> ChildCollections { get; private set; } = new List<CollectionEntity>();
    public ItemEntity? CoverItem { get; private set; }
    public ICollection<ItemEntity> Items { get; private set; } = new List<ItemEntity>();

    private CollectionEntity() { }

    public static CollectionEntity Create(string name, int? parentCollectionId = null, int? coverItemId = null)
    {
        CollectionEntity data = new CollectionEntity
        {
            Name = name,
            ParentCollectionId = parentCollectionId,
            CoverItemId = coverItemId
        };
        
        data.SetModifiedAt();
        
        return data;
    }

    public void Update(string? name, int? parentCollectionId = null, int? coverItemId = null)
    {
        if (!string.IsNullOrEmpty(name)) Name = name;
        ParentCollectionId = parentCollectionId;
        CoverItemId = coverItemId;
        
        SetUpdatedAt();
    }

    public void Delete()
    {
        SetDeletedAt();
    }
}