using CollectionGallery.Domain.Models.Enums;

namespace CollectionGallery.Domain.Models.Entities;

[Table("items")]
public class Item : DBTable
{
    [Column("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [Column("extension")]
    [JsonPropertyName("extension")]
    public string Extension { get; set; } = string.Empty;

    [Column("model_id")]
    [JsonPropertyName("modelId")]
    public int ModelId { get; set; }

    [ForeignKey("ModelId")]
    public Model Models { get; set; } = default!;

    [Column("parent_collection_id")]
    [JsonPropertyName("parentCollectionId")]
    public int? ParentCollectionId { get; set; } = null;

    [ForeignKey("ParentCollectionId")]
    public Collection Collections { get; set; } = default!;

    [Column("size")]
    [JsonPropertyName("size")]
    public FileSize Size { get; set; } = FileSize.Original;

    public List<ItemTags> FileTags { get; set; } = new List<ItemTags>();
    public List<ItemPlatforms> FilePlatforms { get; set; } = new List<ItemPlatforms>();
}

[Table("itemtags")]
public class ItemTags
{
    [Column("item_id")]
    [JsonPropertyName("itemId")]
    public int ItemId { get; set; }
    public Item Item { get; set; } = default!;

    [Column("tag_id")]
    [JsonPropertyName("tagId")]
    public int TagId { get; set; }
    public Tags Tag { get; set; } = default!;
}

[Table("itemplatforms")]
public class ItemPlatforms
{
    [Column("item_id")]
    [JsonPropertyName("itemId")]
    public int ItemId { get; set; }
    public Item Item { get; set; } = default!;

    [Column("platform_id")]
    [JsonPropertyName("platformId")]
    public int PlatformId { get; set; }
    public Platforms Platform { get; set; } = default!;
}