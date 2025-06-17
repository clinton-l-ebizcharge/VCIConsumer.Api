using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Requests;

public class AuthenticationRequest
{
    [JsonPropertyName("client_id")]
    public required string ClientId { get; set; }
    [JsonPropertyName("client_secret")]
    public required string ClientSecret { get; set; }
}
