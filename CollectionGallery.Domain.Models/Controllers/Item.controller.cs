using Microsoft.AspNetCore.Http;

namespace CollectionGallery.Domain.Models.Controllers;

/// <summary>
/// Represents the form data for a file upload operation.
/// </summary>
public class FileUploadForm
{
    /// <summary>
    /// The name of the Model to associated with
    /// </summary>    
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The Id of the collection where the file will be saved.
    /// This value can be null, indicating a default or root collection.
    /// </summary>
    public int? CollectionId { get; set; } = null;

    /// <summary>
    /// The uploaded file itself, encapsulated in an <see cref="IFormFile"/> interface.
    /// </summary>
    public required IFormFile File { get; set; } = null!;

    /// <summary>
    /// The list of tags the file is associated with
    /// </summary>
    public List<int>? Tags { get; set; } = null;

    /// <summary>
    /// The list of platforms the file is associated with
    /// </summary>
    public List<int>? Platforms { get; set; } = null;
}

public class FileUploadResultObject
{
    public required string ContentType { get; set; }
    public required string FileName { get; set; }
    public int CollectionId { get; set; } = 0;
    public required string Extension { get; set; }
    public required string Model { get; set; }
    public required string TraceId { get; set; }
    public List<int>? Tags { get; set; } = null;
    public List<int>? Platforms { get; set; } = null;
}