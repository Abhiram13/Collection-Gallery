using CollectionGallery.InfraStructure.Data.Configurations;
using CollectionGallery.Shared.Models;
using Microsoft.Extensions.Options;

namespace CollectionGallery.InfraStructure.Data.HttpClients;

public class CloudStorageHttpClient : HttpClient
{
    private readonly HttpClient _httpClient;
    private readonly DataSecrets _secrets;
    
    public CloudStorageHttpClient(HttpClient httpClient, IOptions<DataSecrets> secrets)
    {
        _secrets = secrets.Value;
        _httpClient = httpClient;
        
        _httpClient.BaseAddress = new Uri(_secrets.StorageServer);
    }

    public async Task<SignedUrlResponseDto?> GetSignedUrlAsync(GetSignedUrlDto payload)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/storage", payload);
        SignedUrlResponseDto? apiResponse = await response.Content.ReadFromJsonAsync<SignedUrlResponseDto>();

        return apiResponse;
    }
}