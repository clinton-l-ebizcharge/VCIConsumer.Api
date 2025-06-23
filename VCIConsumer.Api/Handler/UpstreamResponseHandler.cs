using System.Text.Json;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Handler;

public static class UpstreamResponseHandler
{
    public static async Task<ApiResponse> HandleFailureAsync(HttpResponseMessage response, ILogger logger, string contextTag)
    {
        var responseBody = response.Content != null
            ? await response.Content.ReadAsStringAsync()
            : string.Empty;

        logger.LogWarning("{ContextTag}: Upstream failed. Status={StatusCode}, Body={Body}",
            contextTag, (int)response.StatusCode, responseBody);

        try
        {
            var envelope = JsonSerializer.Deserialize<BasicErrorEnvelope>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var message = envelope?.Errors?.FirstOrDefault()?.Message;

            return new ApiResponse
            {
                IsSuccess = false,
                StatusCode = response.StatusCode,
                Errors = new List<ErrorResponse>
                {
                    new()
                    {
                        Code = "UpstreamError",
                        Message = message ?? "A generic error occurred while processing the request.",
                        Type = "Upstream"
                    }
                }
            };
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "{ContextTag}: Failed to parse upstream error response. Raw={Json}", contextTag, responseBody);

            return new ApiResponse
            {
                IsSuccess = false,
                StatusCode = response.StatusCode,
                Errors = new List<ErrorResponse>
                {
                    new()
                    {
                        Code = "InvalidErrorPayload",
                        Message = "Received a malformed error payload from upstream service.",
                        Type = "Upstream"
                    }
                }
            };
        }
    }
}

public class BasicErrorEnvelope
{
    public List<BasicError>? Errors { get; set; }
}

public class BasicError
{
    public string? Message { get; set; }
}

