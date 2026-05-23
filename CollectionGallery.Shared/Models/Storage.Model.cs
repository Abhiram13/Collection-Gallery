namespace CollectionGallery.Shared.Models;

public record GetSignedUrlDto
{
    /// <summary>
    /// Original name of the file.
    /// </summary>
    public required string FileName { get; init; }
    
    /// <summary>
    /// Item id the file belongs to.
    /// </summary>
    /// <remarks>This is used to pass in meta data of the file upload and used to link with file and item in DB</remarks>
    public required int ItemId { get; init; }
}

public record SignedUrlResponseDto
{
    public string StorageKey { get; init; }
    public string SignedUrl { get; init; }
}