using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using CollectionGallery.InfraStructure.Data.Constants;

namespace CollectionGallery.InfraStructure.Data.Entities;

public abstract class BaseEntity
{
    [Key]
    [Column(DbTableNames.Base.ID)]
    public int Id { get; set; }

    [Column(DbTableNames.Base.CREATED_AT)]
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    [Column(DbTableNames.Base.UPDATED_AT)]
    public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;

    [Column(DbTableNames.Base.DELETED_AT)] 
    public DateTimeOffset? DeletedAt { get; private set; } = null;
    
    protected void SetModifiedAt()
    {
        DateTimeOffset now =  DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }
    
    protected void SetUpdatedAt()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        UpdatedAt = now;
    }
    
    protected void SetDeletedAt()
    {
        DeletedAt = DateTimeOffset.UtcNow;
    }
}