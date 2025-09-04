using System.ComponentModel.DataAnnotations;

namespace CollectionGallery.Domain.Models.Entities;

public abstract class DBTable
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [Column("created_at")]
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

[Table("models")]
public class Model : DBTable
{
    [Column("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}