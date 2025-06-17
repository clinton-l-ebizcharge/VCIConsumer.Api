using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models;

/// <summary>
/// Represents the successful response body from VeriCheck authentication.
/// </summary>
/// <param name="AccessToken">The OAuth 2.0 access token to be used for subsequent API calls.</param>
/// <param name="ExpiresIn">The lifetime in seconds of the access token.</param>
/// <param name="TokenType">The type of token issued (e.g., "Bearer").</param>
/// <param name="Scope">The scope of the access token.</param>
public record VCIAuthResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_in")] int ExpiresIn,
    [property: JsonPropertyName("token_type")] string TokenType,
    [property: JsonPropertyName("scope")] string Scope
);