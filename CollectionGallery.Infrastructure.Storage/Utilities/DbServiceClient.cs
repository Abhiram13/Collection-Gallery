using System.Text.Json;
using CollectionGallery.Domain.Models.Controllers;
using CollectionGallery.Domain.Models.Services;

namespace CollectionGallery.Infrastructure.Storage.Utilities;

public class DbServiceClient
{
    private readonly HttpClient _httpClient;

    public DbServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("DB_SERVER")!);
    }

    public async Task ValidateUploadAsync(ValidateUploadDto payload)
    {
        HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync("api/items/validate", payload);
        try
        {
            httpResponse.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            string jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            ExceptionMessage exceptionMessage = JsonSerializer.Deserialize<ExceptionMessage>(jsonResponse)!;
            throw new HttpRequestException(message: exceptionMessage.Message, inner: ex, statusCode: System.Net.HttpStatusCode.BadRequest);
        }
    }
    
    public async Task UploadSuccessAsync(UploadSuccessDto payload)
    {
        HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync("api/items/uploadSuccess", payload);
        string jsonResponse = await httpResponse.Content.ReadAsStringAsync();
    }
}