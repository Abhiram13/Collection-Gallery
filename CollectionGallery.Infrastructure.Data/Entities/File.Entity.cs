using System.ComponentModel.DataAnnotations.Schema;
using CollectionGallery.InfraStructure.Data.Enums;

using FileTable = CollectionGallery.InfraStructure.Data.Constants.DbTableNames.File;

namespace CollectionGallery.InfraStructure.Data.Entities;

[Table(FileTable.TABLE_NAME)]
public class CollectionFile : BaseEntity
{
    [Column(FileTable.NAME)]
    public required string Name { get; set; }
    
    [Column(FileTable.EXTENSION)]
    public required string Extension { get; set; }
    
    [Column(FileTable.SIZE)]
    public long Size { get; set; }
    
    [Column(FileTable.MIME)]
    public required string MimeType { get; set; }
    
    [Column(FileTable.BUCKET)]
    public string Bucket { get; set; } = string.Empty;
    
    [Column(FileTable.STORAGE_KEY)]
    public string StorageKey { get; set; } = string.Empty;

    public ItemEntity Item { get; set; } = default!;
}