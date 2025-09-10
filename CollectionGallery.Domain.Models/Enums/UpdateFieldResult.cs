namespace CollectionGallery.Domain.Models.Enums;

public enum UpdateFieldResult
{
    Success = 1,
    NotFound = 2,
    ParentNotFound = 3,
    InternalError = 4
}