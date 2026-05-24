using CollectionGallery.InfraStructure.Storage.Configuration;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Google.Cloud.Storage.Control.V2;
using CollectionGallery.Shared;
using CollectionGallery.Shared.Models;

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
    
    public GoogleStorageService(StorageSecrets secrets)
    {
        _credential = GoogleCredential.FromFile(secrets.GoogleCredentialFile);
        _storageClient = StorageClient.Create(_credential);
        _bucketName = secrets.Bucket;
        _storageControlClient = StorageControlClient.Create();
    }

    public string GenerateSignedUrl(GetSignedUrlDto payload)
    {
        TimeSpan expiresIn = TimeSpan.FromMinutes(5);
        UrlSigner signer = UrlSigner.FromCredential(GoogleCredential.GetApplicationDefault());
        UrlSigner.RequestTemplate template = UrlSigner.RequestTemplate
            .FromBucket(_bucketName)
            .WithHttpMethod(HttpMethod.Put)
            .WithRequestHeaders(new Dictionary<string, IEnumerable<string>>
            {
                { "x-goog-meta-item-id", new string[] { payload.ItemId.ToString() } }
            })
            .WithObjectName(payload.FileName);
        
        string url = signer.Sign(requestTemplate: template, options: UrlSigner.Options.FromDuration(expiresIn));
        return url;
    }

    public StorageObject GetObjectMetadata(string fileName)
    {
        StorageObject result = _storageClient.GetObject(bucket: _bucketName, fileName);

        return result;
    }
}