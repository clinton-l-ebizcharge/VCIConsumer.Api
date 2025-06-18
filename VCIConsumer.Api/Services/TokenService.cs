using Microsoft.Extensions.Options;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Requests;
using VCIConsumer.Api.Models.Responses;
namespace VCIConsumer.Api.Services;

public class TokenService(IOptions<ApiSettings> apiSettings, HttpClient httpClient, ILogger<TokenService> logger) : ITokenService
{
    private AccessToken? _accessToken;
    private readonly IApiSettings _apiSettings = apiSettings.Value;
    private readonly HttpClient httpClient = httpClient;
    private readonly ILogger<TokenService> _logger = logger;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public async Task<string> GetTokenAsync()
    {
        if (_accessToken != null && _accessToken.IsValid)
            return _accessToken.Token; // Return cached token if valid  

        await _semaphore.WaitAsync(); // Make sure this is awaited to prevent deadlocks  
        try
        {
            if (string.IsNullOrEmpty(_apiSettings.ClientId) || string.IsNullOrEmpty(_apiSettings.ClientSecret))
                throw new InvalidOperationException("ClientId or ClientSecret is not configured in API settings.");

            AuthenticationRequest AuthenticationRequest = new AuthenticationRequest
            {
                ClientId = _apiSettings.ClientId,
                ClientSecret = _apiSettings.ClientSecret
            };

            var httpContent = JsonContent.Create(AuthenticationRequest);
            var httpResponseMessage = await httpClient.PostAsync("authentication", httpContent);

            // Ensure the response is not null before accessing its properties  
            var authenticationResponse = await httpResponseMessage.Content.ReadFromJsonAsync<AuthenticationResponse>();
            if (authenticationResponse == null)
                throw new InvalidOperationException("Authentication response is null.");

            _accessToken = new AccessToken()
            {
                Token = authenticationResponse.AccessToken,
                TokenType = authenticationResponse.TokenType,
                CreatedAt = DateTime.Now,
                ExpiresIn = authenticationResponse.ExpiresIn
            };

            return _accessToken.Token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching the access token."); // Use '_logger' to log the error  
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}


