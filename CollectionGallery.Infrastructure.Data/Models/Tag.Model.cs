using System.Text.Json.Serialization;

namespace CollectionGallery.InfraStructure.Data.Tag.Models;

public record InsertTagDto
{
    public required string Name { get; init; }
}