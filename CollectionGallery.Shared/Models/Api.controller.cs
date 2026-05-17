using System.Net;
using System.Text.Json.Serialization;

namespace CollectionGallery.Shared.Models;

public sealed class ApiResponse<T> where T : class
{
    [JsonPropertyName("status_code")]
    public HttpStatusCode StatusCode { get; set; }

    [JsonPropertyName("trace_id")]
    public required string TraceId { get; set; }

    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }

    [JsonPropertyName("result")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Result { get; set; }
}

public sealed class ApiResponse
{
    [JsonPropertyName("status_code")] 
    public HttpStatusCode StatusCode { get; set; }

    [JsonPropertyName("trace_id")] 
    public required string TraceId { get; set; }

    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }
}
