using System.Text.Json.Serialization;

namespace CollectionGallery.InfraStructure.Data.Collection.Models;

[Obsolete(message: "", error: true)]
public class CollectionCreateForm
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("parent_collection")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ParentFolderName { get; set; } = null;
}

public record ParentCollections
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("collection_pic")]
    public string? CollectionPic { get; init; } = string.Empty;
}

public record CollectionDetailsById
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("collection_pic")]
    public string CollectionPic { get; set; } = string.Empty;

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; } = DateTime.Now;

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    [JsonPropertyName("platform")]
    public List<CollectionPlatforms> Platforms { get; set; } = new List<CollectionPlatforms>();

    [JsonPropertyName("child_collections")]
    public List<ChildCollection> Collections { get; set; } = new List<ChildCollection>();

    [JsonPropertyName("items")]
    public List<CollectionItems> Items { get; set; } = new List<CollectionItems>();

    public abstract record CollectionData
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public record CollectionPlatforms : CollectionData
    {
        [JsonPropertyName("icon")]
        public string Icon { get; set; } = string.Empty;
    }

    public record ChildCollection : CollectionData
    {
        [JsonPropertyName("collection_pic")]
        public string CollectionPic { get; set; } = string.Empty;
    }

    public record CollectionItems : CollectionData;
}