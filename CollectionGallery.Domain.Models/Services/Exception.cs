namespace CollectionGallery.Domain.Models.Services;

public class ExceptionMessage
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}