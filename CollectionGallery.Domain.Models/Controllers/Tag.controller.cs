namespace CollectionGallery.Domain.Models.Controllers;

public class TagList
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}