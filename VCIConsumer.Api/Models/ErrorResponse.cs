namespace VCIConsumer.Api.Models;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public class ErrorResponse
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}