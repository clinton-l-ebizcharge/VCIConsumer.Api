using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses;

public class PaymentPostResponse
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("velocity_breached")]
    public bool VelocityBreached { get; set; } = true;

    [JsonPropertyName("company_id")]
    public string CompanyId { get; set; }

    [JsonPropertyName("amount")]
    public double Amount { get; set; } = 0;

    [JsonPropertyName("standard_entry_class")]
    public string StandardEntryClass { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

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
    public WarningCustomer Customer { get; set; }

    [JsonPropertyName("bank_account")]
    public WarningBankAccount BankAccount { get; set; }
}
