using System.Text.Json.Serialization;

namespace CollectionGallery.InfraStructure.Data.Collection.Models;

public class InsertCollectionDto
{
    public required string Name { get; init; }
    public int? ParentCollectionId { get; init; } = null;
    public int? CoverItemId { get; init; } = null;
}

public record ParentCollection
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
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