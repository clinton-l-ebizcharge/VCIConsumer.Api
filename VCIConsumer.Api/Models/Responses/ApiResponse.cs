using System.Net;
using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses;

public class ApiResponse
{
    public bool IsSuccess { get; set; } = false;
    public required object Result { get; set; } 
    public HttpStatusCode StatusCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? CorrelationId { get; set; }

    [JsonPropertyName("errors")]
    public List<ErrorResponse> Errors { get; set; } = new List<ErrorResponse>();
}
