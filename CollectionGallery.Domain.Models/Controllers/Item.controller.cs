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
    /// The path of the folder where the file will be saved.
    /// This value can be null, indicating a default or root directory.
    /// </summary>
    public int? Folder { get; set; } = null;

    /// <summary>
    /// The uploaded file itself, encapsulated in an <see cref="IFormFile"/> interface.
    /// </summary>
    public required IFormFile File { get; set; } = null!;
}

public class FileUploadResultObject
{
    public required string ContentType { get; set; }
    public required string FileName { get; set; }
    public required string Extension { get; set; }
    public required string ModelName { get; set; }
    public required string TraceId { get; set; }
}