using System.Text.Json.Serialization;
using static VCIConsumer.Api.Models.Enums.Enums;

namespace VCIConsumer.Api.Models.Responses;

public class PaymentPostWithTokenResponse
{
    [JsonPropertyName("warnings")]
    public List<ErrorResponse> Warnings { get; set; }
    [JsonPropertyName("velocity_breached")]
    public bool VelocityBreached { get; set; }
    [JsonPropertyName("company_id")]
    public string CompanyId { get; set; }
    [JsonPropertyName("amount")]
    public double Amount { get; set; } = 0;
    [JsonPropertyName("standard_entry_class")]
    public StandardEntryClass StandardEntryClass { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("addenda")]
    public string Addenda { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; }
    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }
    [JsonPropertyName("hrid")]
    public string HRId { get; set; }
    [JsonPropertyName("uuid")]
    public string UUId { get; set; }
    [JsonPropertyName("correlation_id")]
    public string CorrelationId { get; set; }
    [JsonPropertyName("customer")]
    public PaymentPostCustomerResponse Customer { get; set; }
    [JsonPropertyName("idempotency_key")]
    public string IdempotencyKey { get; set; }
    [JsonPropertyName("api_rate_limits")]
    public ApiRateLimitsResponse ApiRateLimits { get; set; }

}
