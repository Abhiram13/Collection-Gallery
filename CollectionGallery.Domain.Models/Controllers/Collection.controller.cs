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
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CollectionPic { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public List<CollectionPlatforms> Platforms { get; set; } = new List<CollectionPlatforms>();
    public List<ChildCollection> Collections { get; set; } = new List<ChildCollection>();
    public List<CollectionItems> Items { get; set; } = new List<CollectionItems>();

    public class CollectionPlatforms
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    public class ChildCollection
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CollectionPic { get; set; } = string.Empty;
    }

    public class CollectionItems
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
    }
}