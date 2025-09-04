namespace CollectionGallery.Domain.Models.Controllers;

public class FolderCreateForm
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("parentFolder")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ParentFolderName { get; set; } = null;
}