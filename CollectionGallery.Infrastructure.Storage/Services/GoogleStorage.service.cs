using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Google.Cloud.Storage.Control.V2;

namespace CollectionGallery.Infrastructure.Storage.Services;

public class GoogleStorageService
{
    /// <summary>
    /// The Google credential used for authentication with Google Cloud services.
    /// This credential grants the necessary permissions to interact with Google Cloud Storage.
    /// </summary>
    protected readonly GoogleCredential _credential;

    /// <summary>
    /// The client used for performing common operations on Google Cloud Storage,
    /// such as uploading, downloading, and managing objects.
    /// </summary>
    protected readonly StorageClient _storageClient;

    /// <summary>
    /// The client used for advanced, programmatic control over Google Cloud Storage buckets,
    /// including creating, updating, and deleting bucket-level settings.
    /// </summary>
    protected readonly StorageControlClient _storageControlClient;

    /// <summary>
    /// The name of the Google Cloud Storage bucket used by this class.
    /// </summary>
    protected readonly string _bucketName;

    // TODO: Update with ENV vars or with Secret manager 
    public GoogleStorageService()
    {
        _credential = GoogleCredential.FromFile(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
        _storageClient = StorageClient.Create(_credential);
        // _storageClient = StorageClient.Create();
        _bucketName = "models-management-bucket";
        _storageControlClient = StorageControlClient.Create();
    }    
}