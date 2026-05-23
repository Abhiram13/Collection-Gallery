namespace CollectionGallery.InfraStructure.Data.File.Models;

public record InsertCollectionFileDto
{
    public string Name { get; init; } = string.Empty;
    public string Extension { get; init; } = string.Empty;
    public long Size { get; init; }
    public string MimeType { get; init; } = string.Empty;
    public string Bucket { get; init; } = string.Empty;
    public int ItemId { get; init; }
}

public record StrorageUploadObjectDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("selfLink")]
    public string SelfLink { get; init; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("bucket")]
    public string Bucket { get; init; } = string.Empty;
    
    [JsonPropertyName("contentType")]
    public string ContentType { get; init; } = string.Empty;

    [JsonPropertyName("size")] 
    public string Size { get; init; } = string.Empty;
    
    [JsonPropertyName("md5Hash")]
    public string Hash { get; init; } = string.Empty;
    
    [JsonPropertyName("mediaLink")]
    public string MediaLink { get; init; } = string.Empty;
    
    [JsonPropertyName("metadata")]
    public MetaDataDto MetaData { get; init; } = new MetaDataDto();

    public record MetaDataDto
    {
        [JsonPropertyName("item-id")]
        public string ItemId { get; init; } = string.Empty;
    }
}