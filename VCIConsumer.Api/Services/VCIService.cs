using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Models.Request;

namespace VCIConsumer.Api.Services;

public class VCIService(IOptions<ApiSettings> apiSettings, IHttpClientFactory httpClientFactory) : ServiceBase, IVCIService
{
    public async Task<IResult> GetTokenAsync(AuthenticationRequest request)
    {
        try
        {
            // Ensure defaults if no request object is provided
            var requestBody = new AuthenticationRequest(apiSettings.Value.ClientId ?? string.Empty, apiSettings.Value.ClientSecret ?? string.Empty);

            if (string.IsNullOrEmpty(requestBody.ClientId) || string.IsNullOrEmpty(requestBody.ClientSecret))
                return Results.BadRequest("ClientId and ClientSecret must be provided in the request body or configuration.");

            // Ensure ClientName is not null or empty before calling CreateClient
            if (string.IsNullOrEmpty(apiSettings.Value.ClientName))
                return Results.Problem(title: "Configuration Error", detail: "ClientName is not configured in ApiSettings.");

            var client = httpClientFactory.CreateClient(apiSettings.Value.ClientName);
           
            // Create the HttpRequestMessage
            string requestUri = $"{client.BaseAddress}/authentication";
            var htttpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri);
            htttpRequest.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            AddRequestHeaders(htttpRequest, apiSettings);

            LogRequest(requestBody, htttpRequest);

            // Send the POST request to the authentication endpoint.
            var response = await client.SendAsync(htttpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            LogResponse(response, responseContent);

            if (response.IsSuccessStatusCode)
            {
                return Results.Ok(responseContent); // Return 200 OK with the token details.
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                // Use Results.Problem for structured error responses
                return Results.Problem(
                    statusCode: (int)response.StatusCode,
                    title: "Failed to obtain access token",
                    detail: "The VeriCheck API returned an error.",
                    extensions: new Dictionary<string, object?>
                    {
                        {"VciResponse", errorContent}
                    });
            }
        }
        catch (HttpRequestException ex)
        {
            return Results.Problem(title: "Network Error", detail: $"An error occurred while communicating with the VeriCheck API: {ex.Message}");
        }
        catch (JsonException ex)
        {
            return Results.Problem(title: "Response Parsing Error", detail: $"Failed to parse VeriCheck API response: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Results.Problem(title: "Internal Server Error", detail: $"An unexpected error occurred: {ex.Message}");
        }

        static void AddRequestHeaders(HttpRequestMessage request, IOptions<ApiSettings> apiSettings)
        {
            request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
            request.Headers.Add("VeriCheck-Version", apiSettings.Value.VeriCheckVersion);            
        }
    }

    private static void LogResponse(HttpResponseMessage response, string responseContent)
    {
        // **LOG FULL RESPONSE AFTER RECEIVING**
        Console.WriteLine("=== HTTP RESPONSE ===");
        Console.WriteLine($"Status Code: {response.StatusCode}");
        Console.WriteLine("Headers:");
        foreach (var header in response.Headers)
        {
            Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
        }
        Console.WriteLine("Body:");
        Console.WriteLine(responseContent);
        Console.WriteLine("====================");
    }

    private static void LogRequest(global::System.Object payload, HttpRequestMessage request)
    {
        // **LOG FULL REQUEST BEFORE SENDING**
        Console.WriteLine("=== HTTP REQUEST ===");
        Console.WriteLine($"Method: {request.Method}");
        Console.WriteLine($"URL: {request.RequestUri}");
        Console.WriteLine("Headers:");
        foreach (var header in request.Headers)
        {
            Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
        }
        Console.WriteLine("Body:");
        Console.WriteLine(payload);
        Console.WriteLine("====================");
    }
}
