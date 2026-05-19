using System.Net;
using System.Text.Json.Serialization;

namespace CollectionGallery.Shared.Models;

public sealed record ApiResponse<T> where T : class
{
    public HttpStatusCode StatusCode { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TraceId { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Result { get; set; }
}

public sealed record ApiResponse
{
    public HttpStatusCode StatusCode { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TraceId { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string Message { get; set; }
}