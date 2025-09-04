using CollectionGallery.Infrastructure.Storage.Utilities;

namespace CollectionGallery.Infrastructure.Storage.Services;

public class ItemService : GoogleStorageService
{
    private readonly ImageService _imageService;

    public ItemService(ImageService imageService)
    {
        _imageService = imageService;
    }

    /// <summary>
    /// Uploads a file to Google Cloud Storage using file metadata and <see cref="IFormFile"/>.
    /// </summary>
    /// <param name="data">The <see cref="FileMeta"/> object containing the file's metadata and content.</param>
    /// <returns>A <see cref="StorageObject"/> representing the uploaded file on Google Cloud Storage.</returns>
    public async Task<StorageObject> UploadFileAsync(FileMeta data)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            await data.File.OpenReadStream().CopyToAsync(memoryStream);
            StorageObject storageObject = await UploadStreamAsync(data, memoryStream);

            return storageObject;
        }
    }

    /// <summary>
    /// Uploads a file to Google Cloud Storage using a byte array.
    /// </summary>
    /// <param name="data">The <see cref="FileMeta"/> object containing the file's metadata and content.</param>
    /// <param name="bytes">The byte array containing the file's content.</param>
    /// <returns>A <see cref="StorageObject"/> representing the uploaded file on Google Cloud Storage.</returns>
    public async Task<StorageObject> UploadFileAsync(FileMeta meta, byte[] bytes)
    {
        using (MemoryStream stream = new MemoryStream(bytes))
        {
            StorageObject storageObject = await UploadStreamAsync(meta, stream);
            return storageObject;
        }
    }

    /// <summary>
    /// Uploads a stream of data to a specified Google Cloud Storage location.
    /// This is the core method used for all file uploads.
    /// </summary>
    /// <param name="meta">The <see cref="FileMeta"/> object containing the file's metadata and content.</param>
    /// <param name="stream">The stream containing the file's content to be uploaded.</param>
    /// <returns>A <see cref="StorageObject"/> representing the uploaded file on Google Cloud Storage.</returns>
    private async Task<StorageObject> UploadStreamAsync(FileMeta meta, MemoryStream stream)
    {
        StorageObject obj = new StorageObject
        {
            Bucket = _bucketName,
            Name = "",
            ContentType = meta.ContentType,
        };

        StorageObject sobj = await _storageClient.UploadObjectAsync(destination: obj, source: stream);
        return sobj;
    }
}