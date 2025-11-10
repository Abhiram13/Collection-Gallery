using Microsoft.AspNetCore.Mvc;

namespace CollectionGallery.Domain.Models.Controllers;

[Obsolete]
public class CollectionCreateForm
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("parent_collection")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ParentFolderName { get; set; } = null;
}

public class ParentCollections
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("collection_pic")]
    public string? CollectionPic { get; init; } = string.Empty;
}

public class CollectionDetailsById
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

    [JsonPropertyName("child_collections")]
    public List<ChildCollection> Collections { get; set; } = new List<ChildCollection>();

    public class ChildCollection
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("collection_pic")]
        public string CollectionPic { get; set; } = string.Empty;
    }
}

public class ItemsByCollectionId
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public record class CreateCollectionDto
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("parentCollectionId")]
    public int? ParentCollectionId { get; init; } = null;
}

public record class CreateItemByCollectionIdDto
{
    [JsonPropertyName("file_name")]
    [FromForm(Name = "file_name")]
    public required string FileName { get; init; }

    [JsonPropertyName("tags")]
    public string[] Tags { get; init; }
}