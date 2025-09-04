namespace CollectionGallery.Domain.Models.Entities;

[Table("tags")]
public class Tags : DBTable
{
    [Column("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    public List<ItemTags> FileTags { get; set; } = new List<ItemTags>();
}