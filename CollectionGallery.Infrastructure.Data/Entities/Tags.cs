using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CollectionGallery.InfraStructure.Data.Entities;

[Table("tags")]
public class Tags : BaseEntity
{
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    public List<ItemTags> ItemTags { get; init; } = new List<ItemTags>();
}