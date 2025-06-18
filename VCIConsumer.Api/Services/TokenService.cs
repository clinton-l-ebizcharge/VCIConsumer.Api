using Microsoft.Extensions.Options;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Models;

namespace VCIConsumer.Api.Services;

public class TokenService(IOptions<IApiSettings> apiSettings, IHttpClientFactory httpClientFactory)
{
    private AccessToken? _accessToken;
    private readonly IApiSettings _apiSettings = apiSettings.Value;

    public async Task<string> GetAccessTokenAsync()
    {
        if (_accessToken != null && _accessToken.IsValid)
            return _accessToken.Token; // Return cached token if valid

        var client = httpClientFactory.CreateClient("VCIApi");
        var response = await client.PostAsync("authentication", null); // Replace with actual request

        var tokenAsString = await response.Content.ReadFromJsonAsync<string>();
        if (tokenAsString == null) // Handle null case explicitly
            throw new InvalidOperationException("Failed to retrieve access token from the response.");

        _accessToken = new AccessToken()
        {
            Token = tokenAsString,
            TokenType = "Bearer",
            CreatedAt = DateTime.Now,
            ExpiresIn = 3600
        };

        return tokenAsString;
    }
}
