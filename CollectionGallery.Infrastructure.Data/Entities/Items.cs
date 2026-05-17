using System.ComponentModel.DataAnnotations.Schema;
using CollectionGallery.InfraStructure.Data.Enums;

namespace CollectionGallery.InfraStructure.Data.Entities;

[Table("items")]
public class Item : BaseEntity
{
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("extension")]
    public string Extension { get; set; } = string.Empty;

    [Column("model_id")]
    public int? ModelId { get; set; }

    [ForeignKey("ModelId")]
    public Model Models { get; set; } = default!;

    [Column("parent_collection_id")]
    public int? ParentCollectionId { get; set; } = null;

    [ForeignKey("ParentCollectionId")]
    public Collection Collections { get; set; } = default!;

    [Column("size")]
    public FileSize Size { get; set; } = FileSize.Original;

    public List<ItemTags> ItemTags { get; set; } = new List<ItemTags>();
}

[Table("item_tags")]
public class ItemTags
{
    [Column("item_id")]
    public int ItemId { get; set; }
    
    public Item Item { get; set; } = default!;

    [Column("tag_id")]
    public int TagId { get; set; }
    
    public Tags Tag { get; set; } = default!;
}