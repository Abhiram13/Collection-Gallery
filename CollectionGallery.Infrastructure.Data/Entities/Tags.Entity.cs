using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using TagTable = CollectionGallery.InfraStructure.Data.Constants.DbTableNames.Tag;

namespace CollectionGallery.InfraStructure.Data.Entities;

[Table(TagTable.TABLE_NAME)]
public class TagEntity : BaseEntity
{
    [Column(TagTable.NAME)]
    public string Name { get; private set; } = string.Empty;

    public ICollection<ItemTags> ItemTags { get; set; } = new List<ItemTags>();
    
    private TagEntity() { }

    public static TagEntity Create(string name)
    {
        TagEntity tag = new TagEntity
        {
            Name = name.ToLower().Trim()
        };
        
        tag.SetModifiedAt();
        
        return tag;
    }
}