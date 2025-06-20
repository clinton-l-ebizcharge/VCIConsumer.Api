using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Requests;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Services;

public class AuthenticationService(IHttpClientFactory httpClientFactory, ILogger<AuthenticationService> logger) : IAuthenticationService
{
    private AccessToken? _accessToken;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<AuthenticationService> _logger = logger;

    public async Task<string> GetAccessTokenAsync(AuthenticationRequest request)
    {
        if (_accessToken != null && _accessToken.IsValid)
            return _accessToken.Token; // Return cached token if valid  

        var client = _httpClientFactory.CreateClient("VCIApi");
        var response = await client.PostAsync("authentication", null); // Replace with actual request  
        var responseAsJson = await response.Content.ReadFromJsonAsync<string>();

        if (string.IsNullOrEmpty(responseAsJson))
        {
            _logger.LogError("Failed to retrieve access token: response content is null.");
            throw new InvalidOperationException("Failed to retrieve access token: response content is null.");
        }

        _accessToken = new AccessToken()
        {
            Token = responseAsJson,
            TokenType = "Bearer",
            ExpiresAt = DateTime.UtcNow.AddHours(1), 
            CreatedAt = DateTime.UtcNow
        };

        return _accessToken.Token;
    }
}
