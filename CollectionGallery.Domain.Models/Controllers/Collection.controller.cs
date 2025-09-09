namespace CollectionGallery.Domain.Models.Controllers;

[Obsolete]
public class CollectionCreateForm
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("parentCollection")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ParentFolderName { get; set; } = null;
}

public class ParentCollections
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("collectionPic")]
    public string? CollectionPic { get; init; } = string.Empty;
}

public class CollectionDetailsById
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("collectionPic")]
    public string CollectionPic { get; set; } = string.Empty;

    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAt { get; set; } = DateTime.Now;

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    [JsonPropertyName("platform")]
    public List<CollectionPlatforms> Platforms { get; set; } = new List<CollectionPlatforms>();

    [JsonPropertyName("childCollections")]
    public List<ChildCollection> Collections { get; set; } = new List<ChildCollection>();

    [JsonPropertyName("items")]
    public List<CollectionItems> Items { get; set; } = new List<CollectionItems>();

    public class CollectionPlatforms
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("icon")]
        public string Icon { get; set; } = string.Empty;
    }

    public class ChildCollection
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("collectionPic")]
        public string CollectionPic { get; set; } = string.Empty;
    }

    public class CollectionItems
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}