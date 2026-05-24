using System.Text.Json.Serialization;

namespace CollectionGallery.InfraStructure.Data.Item.Models;

public record ItemInsertDto
{
    public required string Name { get; set; }
    public required string FileName { get; set; } = string.Empty;
    public int? CollectionId { get; set; } = null;
    public string? Tags { get; set; } = null;
}