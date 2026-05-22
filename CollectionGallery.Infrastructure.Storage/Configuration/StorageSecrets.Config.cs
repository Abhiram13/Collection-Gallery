namespace CollectionGallery.InfraStructure.Storage.Configuration;

public record StorageSecrets
{
    public required string Bucket { get; init; }
    
    [ConfigurationKeyName("GOOGLE_CLOUD_PROJECT_ID")]
    public required string GoogleCloudProjectId { get; init; }
    
    [ConfigurationKeyName("GOOGLE_APPLICATION_CREDENTIALS")]
    public required string GoogleCredentialFile { get; init; }
}