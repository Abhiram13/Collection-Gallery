namespace CollectionGallery.Shared.Models;

public record GetSignedUrlDto
{
    public required string FileName { get; init; }
}

public record SignedUrlResponseDto
{
    public string StorageKey { get; init; }
    public string SignedUrl { get; init; }
}