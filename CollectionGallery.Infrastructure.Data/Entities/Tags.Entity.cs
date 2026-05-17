using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using TagTable = CollectionGallery.InfraStructure.Data.Constants.DbTableNames.Tag;

namespace CollectionGallery.InfraStructure.Data.Entities;

[Table(TagTable.TABLE_NAME)]
public class Tags : BaseEntity
{
    [Column(TagTable.NAME)]
    public string Name { get; set; } = string.Empty;

    public ICollection<ItemTags> ItemTags { get; set; } = new List<ItemTags>();
}