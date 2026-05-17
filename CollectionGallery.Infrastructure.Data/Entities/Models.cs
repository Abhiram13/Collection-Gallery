using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CollectionGallery.InfraStructure.Data.Entities;

public abstract class BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; init; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

[Table("models")]
public class Model : BaseEntity
{
    [Column("name")]
    public required string Name { get; set; } = string.Empty;
}