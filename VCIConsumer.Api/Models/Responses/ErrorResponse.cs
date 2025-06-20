namespace VCIConsumer.Api.Models.Responses;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public class ErrorResponse
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    [JsonPropertyName("message")]
    public required string Message { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }
}