using VCIConsumer.Api.Models;

namespace VCIConsumer.Api.Services;

public class TokenService
{
    private AccessToken _accessToken;
    private readonly IHttpClientFactory _httpClientFactory;

    public TokenService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        if (_accessToken != null && _accessToken.IsValid)
            return _accessToken.Token; // Return cached token if valid

        var client = _httpClientFactory.CreateClient("VCIApi");
        var response = await client.PostAsync("authentication", null); // Replace with actual request

        var tokenAsString = await response.Content.ReadFromJsonAsync<string>();
        _accessToken = new AccessToken()
        {
            Token = tokenAsString,
            TokenType = "Bearer",
            CreatedAt = DateTime.Now,
            ExpiresIn = 3600
        };

        return tokenAsString!;
    }
}
