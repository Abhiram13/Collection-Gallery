using System.ComponentModel.DataAnnotations.Schema;
using CollectionGallery.InfraStructure.Data.Enums;

using FileTable = CollectionGallery.InfraStructure.Data.Constants.DbTableNames.File;

namespace CollectionGallery.InfraStructure.Data.Entities;

[Table(FileTable.TABLE_NAME)]
public class CollectionFile : BaseEntity
{
    [Column(FileTable.NAME)]
    public string Name { get; private set; }
    
    [Column(FileTable.EXTENSION)]
    public string Extension { get; private set; }
    
    [Column(FileTable.SIZE)]
    public long Size { get; private set; }
    
    [Column(FileTable.MIME)]
    public string MimeType { get; private set; }
    
    [Column(FileTable.BUCKET)]
    public string Bucket { get; private set; } = string.Empty;
    
    [Column(FileTable.STORAGE_KEY)]
    public string StorageKey { get; private set; } = string.Empty;
    
    [Column(FileTable.ITEM_ID)]
    public int ItemId { get; private set; }

    public ItemEntity Item { get; set; } = default!;
    
    private CollectionFile() { }

    public static CollectionFile Create(string name, string extension, long size, string mimeType, string bucket, int itemId, string? storageKey = null)
    {
        CollectionFile file = new CollectionFile
        {
            Name = name,
            Extension =  extension,
            Size = long.Parse(size.ToString()),
            MimeType =  mimeType,
            Bucket = bucket,
            ItemId = itemId,
            StorageKey = "storageKey"
        };
        
        file.SetModifiedAt();
        
        return file;
    }
}