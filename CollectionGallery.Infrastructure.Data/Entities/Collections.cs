using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CollectionGallery.InfraStructure.Data.Entities;

[Table("collections")]
public class Collection : BaseEntity
{
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("parent_collection_id")]
    [ForeignKey("collections")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ParentCollectionId { get; set; } = null;

    [Column("collection_pic")]
    public string? CollectionPic { get; set; } = null;
}