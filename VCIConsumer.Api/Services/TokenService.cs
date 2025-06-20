using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Requests;
using VCIConsumer.Api.Models.Responses;
namespace VCIConsumer.Api.Services;

public class TokenService(IOptions<ApiSettings> apiSettings, IHttpClientFactory httpClientFactory, ILogger<TokenService> logger) : ITokenService
{
    private AccessToken? _accessToken;
    private readonly IApiSettings _apiSettings = apiSettings.Value;
    private readonly ILogger<TokenService> _logger = logger;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public async Task<string> GetTokenAsync()
    {
        if (_accessToken != null && _accessToken.IsValid)
        {
            _logger.LogInformation("Access token retrieved from memory.");
            return _accessToken.Token; 
        }

        await _semaphore.WaitAsync(); // Make sure this is awaited to prevent deadlocks
        try
        {
            if (string.IsNullOrEmpty(_apiSettings.ClientId) || string.IsNullOrEmpty(_apiSettings.ClientSecret))
                throw new InvalidOperationException("ClientId or ClientSecret is not configured in API settings.");

            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_apiSettings.BaseUrl);

            httpClient.DefaultRequestHeaders.Add("Idempotency-Key", Guid.NewGuid().ToString());
            var vericheckVersion = _apiSettings.VericheckVersion ?? "1.13"; // Default to "1.13" if not set in configuration
            httpClient.DefaultRequestHeaders.Add("VeriCheck-Version", vericheckVersion);

            var authRequest = new AuthenticationRequest
            {
                ClientId = _apiSettings.ClientId,
                ClientSecret = _apiSettings.ClientSecret
            };

            var json = JsonSerializer.Serialize(authRequest);
            var request = new HttpRequestMessage(HttpMethod.Post, "authentication")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await httpClient.SendAsync(request);
            httpResponseMessage.EnsureSuccessStatusCode();

            var authenticationResponse = await httpResponseMessage.Content.ReadFromJsonAsync<AuthenticationResponse>();

            if (authenticationResponse == null)
            {
                throw new InvalidOperationException("Authentication response is null.");
            }

            _accessToken = new AccessToken()
            {
                Token = authenticationResponse.AccessToken,
                TokenType = authenticationResponse.TokenType,
                CreatedAt = DateTime.Now.ToUniversalTime(),
                ExpiresAt = authenticationResponse.ExpirationDateTime
            };

            _logger.LogInformation($"Issued new access token via /authentication at {_accessToken.CreatedAt}");
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


