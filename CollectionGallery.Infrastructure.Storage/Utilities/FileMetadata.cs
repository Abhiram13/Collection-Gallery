namespace CollectionGallery.Infrastructure.Storage.Utilities;

/// <summary>
/// Represents the metadata and content of an uploaded file.
/// This class holds information such as the file's name, size, type,
/// and its destination path.
/// </summary>
public class FileMeta
{
    /// <summary>
    /// The uploaded file itself, encapsulated in the <see cref="IFormFile"/> interface, which provides access to the file's contents and properties.
    /// </summary>
    public IFormFile File { get; init; }

    /// <summary>
    /// The original name of the file as it was uploaded by the user (e.g., "my-report.jpg").
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// The MIME type of the uploaded file (e.g., "application/pdf" or "image/jpeg"), determined by the client during upload.
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// The size of the uploaded file in bytes.
    /// </summary>
    public long SizeInBytes { get; set; }

    /// <summary>
    /// The size of the uploaded file converted to kilobytes (KB) for easier display and readability.
    /// </summary>
    public double SizeInKB { get; set; }

    /// <summary>
    /// Gets or sets the file extension.
    /// </summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileMeta"/> class with the uploaded file
    /// and the folder path where it will be saved.
    /// </summary>
    /// <param name="file">The <see cref="IFormFile"/> representing the uploaded file.</param>
    /// <param name="folderPath">The path of the directory where the file will be saved. Can be null.</param>
    public FileMeta(IFormFile file)
    {
        File = file;
        FileName = File.FileName;
        FetchMetaData();
    }

    private void FetchMetaData()
    {        
        ContentType = File.ContentType;
        SizeInBytes = File.Length;
        Extension = Path.GetExtension(File.FileName);
        SizeInKB = GetSizeInKB();
    }

    private double GetSizeInKB()
    {
        const int ONE_KB = 1024;
        double size = File.Length / ONE_KB;

        return size;
    }
}