using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Services;

public class VCIProcessingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiSettings _settings;

    public VCIProcessingService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> settings)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
    }

    public async Task<IResult> GetAccessTokenAsync()
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_settings.BaseUrl);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var payload = new { client_id = _settings.ClientId, client_secret = _settings.ClientSecret };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("authentication", content);

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var accessToken = JsonSerializer.Deserialize<AccessToken>(jsonResponse);

            return new IResult
            {
                Data = accessToken ?? throw new InvalidOperationException("AccessToken deserialization failed."),
                JSON = jsonResponse,
                Errors = Array.Empty<ErrorMessage>()
            };
        }
        else
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            return new IResult
            {
                Data = null!,
                JSON = errorResponse,
                Errors = new[] { new ErrorMessage { code = "", type = "", message = "Failed to retrieve access token." } }
            };
        }
    }

    // Add other methods as needed, using DI and instance members
}