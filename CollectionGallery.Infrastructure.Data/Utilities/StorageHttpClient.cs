namespace CollectionGallery.Infrastructure.Data;

public class StorageHttpClient
{
    private readonly HttpClient _client;

    public StorageHttpClient(HttpClient httpClient)
    {
        _client = httpClient;
    }

    public async Task<string> CallTestApiAsync()
    {
        HttpResponseMessage httpResponse = await _client.GetAsync("/item/");
        Console.WriteLine("Status code is --> {0}", httpResponse.StatusCode);
        httpResponse.EnsureSuccessStatusCode();

        return await httpResponse.Content.ReadAsStringAsync();
    }
}