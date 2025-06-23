using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses;

public class PaymentDetailResponse
{
    [JsonPropertyName("uuid")]
    public string UUId { get; set; }

    [JsonPropertyName("hrid")]
    public string HRId { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; } = 0;

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("addenda")]
    public string Addenda { get; set; }

    [JsonPropertyName("standard_entry_class")]
    public string StandardEntryClass { get; set; }

    [JsonPropertyName("customer")]
    public PaymentCustomerResponse Customer { get; set; }

    [JsonPropertyName("bank_account")]
    public PaymentBankAccountResponse BankAccount { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }

    [JsonPropertyName("originated_at")]
    public string OriginatedAt { get; set; }

    [JsonPropertyName("settled_at")]
    public string SettledAt { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("origination_scheduled_for")]
    public string OriginationScheduledFor { get; set; }

    [JsonPropertyName("settlement_scheduled_for")]
    public string SettlementScheduledFor { get; set; }

    [JsonPropertyName("refunded_amount")]
    public decimal RefundedAmount { get; set; } = 0;
}
