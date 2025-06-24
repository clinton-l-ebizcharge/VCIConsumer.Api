using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Extensions;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Requests;
using VCIConsumer.Api.Models.Responses;
using VCIConsumer.Api.Services;

public class TokenService(
    IOptions<ApiSettings> apiSettings,
    IHttpClientFactory httpClientFactory,
    ILogger<TokenService> logger,
    ITimeProvider clock) : ITokenService
{
    private AccessToken? _accessToken;
    private readonly IApiSettings _apiSettings = apiSettings.Value;
    private readonly ILogger<TokenService> _logger = logger;
    private readonly ITimeProvider _clock = clock;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task<string> GetTokenAsync()
    {
        if (_accessToken is not null && !_accessToken.IsExpired(_clock))
        {
            _logger.LogInformation("Access token retrieved from memory.");
            return _accessToken.Token;
        }

        await _semaphore.WaitAsync();
        try
        {
            if (string.IsNullOrEmpty(_apiSettings.ClientId) || string.IsNullOrEmpty(_apiSettings.ClientSecret))
                throw new InvalidOperationException("ClientId or ClientSecret is not configured.");

            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_apiSettings.BaseUrl);

            var authRequest = new AuthenticationRequest
            {
                ClientId = _apiSettings.ClientId,
                ClientSecret = _apiSettings.ClientSecret
            };

            var json = JsonSerializer.Serialize(authRequest);
            var vericheckVersion = _apiSettings.VericheckVersion ?? "1.13";

            var request = new HttpRequestMessage(HttpMethod.Post, "authentication")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
            request.Headers.Add("VeriCheck-Version", vericheckVersion);

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
            if (authResponse == null)
                throw new InvalidOperationException("Authentication response is null.");

            _accessToken = new AccessToken
            {
                Token = authResponse.AccessToken,
                TokenType = authResponse.TokenType,
                CreatedAt = _clock.UtcNow,
                ExpiresAt = authResponse.ExpirationDateTime.AsUtc()
            };

            _logger.LogInformation("Issued new access token at {CreatedAt}, Expires at {ExpiresAt}", _accessToken.CreatedAt, _accessToken.ExpiresAt);
            _logger.LogDebug("Token {token}", _accessToken.Token);

            return _accessToken.Token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching access token.");
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
