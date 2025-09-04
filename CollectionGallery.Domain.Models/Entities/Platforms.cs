namespace CollectionGallery.Domain.Models.Entities;

[Table("platforms")]
public class Platforms : DBTable
{
    [Column("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [Column("icon")]
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;

    public List<ItemPlatforms> FilePlatforms { get; set; } = new List<ItemPlatforms>();
}