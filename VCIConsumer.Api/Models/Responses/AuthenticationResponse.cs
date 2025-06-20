using System.Text.Json.Serialization;
using VCIConsumer.Api.Converters;

namespace VCIConsumer.Api.Models.Responses;

public class AuthenticationResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }
    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }
    [JsonPropertyName("expires_in")]
    [JsonConverter(typeof(VericheckDateTimeConverter))]
    public DateTime ExpirationDateTime  { get; set; }
}


