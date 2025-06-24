using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses;

public class ApiRateLimitsResponse
{
    [JsonPropertyName("api_rate_limits_maximum_count")]
    public int MaximumCount { get; set; } = 0;

    [JsonPropertyName("api_rate_limits_current_count")]
    public int CurrentCount { get; set; } = 0;
}

