namespace CollectionGallery.Domain.Models.Entities;

[Table("collections")]
public class Collection : DBTable
{
    [Column("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [Column("parent_collection_id")]
    [ForeignKey("collections")]
    [JsonPropertyName("parentCollectionId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ParentCollectionId { get; set; } = null;

    [Column("collection_pic")]
    [JsonPropertyName("collectionPic")]
    public string? CollectionPic { get; set; } = null;
}