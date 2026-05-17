using System.ComponentModel.DataAnnotations.Schema;

namespace CollectionGallery.InfraStructure.Data.Entities;

[Table("platforms")]
public class Platforms : BaseEntity
{
    [Column("name")]
    public string Name { get; init; } = string.Empty;

    [Column("icon")]
    public string Icon { get; init; } = string.Empty;

    public List<ItemPlatforms> FilePlatforms { get; set; } = new List<ItemPlatforms>();
}