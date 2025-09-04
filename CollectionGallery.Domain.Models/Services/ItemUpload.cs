namespace CollectionGallery.Domain.Models.Services;

public class ItemUploadData
{
    public string ModelName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
}