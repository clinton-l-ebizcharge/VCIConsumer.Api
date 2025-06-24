using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses
{
    public class PaymentListPaymentDetailResponse
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; } = default!;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; } = 0;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("addenda")]
        public string? Addenda { get; set; }

        [JsonPropertyName("standard_entry_class")]
        public string? StandardEntryClass { get; set; }

        [JsonPropertyName("customer")]
        public CustomerSummary Customer { get; set; } = new();

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("originated_at")]
        public DateTime? OriginatedAt { get; set; }

        [JsonPropertyName("settled_at")]
        public DateTime? SettledAt { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("origination_scheduled_for")]
        public DateTime? OriginationScheduledFor { get; set; }

        [JsonPropertyName("settlement_scheduled_for")]
        public DateTime? SettlementScheduledFor { get; set; }

        [JsonPropertyName("originated_at_window")]
        public string? OriginatedAtWindow { get; set; }

        [JsonPropertyName("settled_at_window")]
        public string? SettledAtWindow { get; set; }

        [JsonPropertyName("warnings")]
        public List<Warning> Warnings { get; set; } = new();

        [JsonPropertyName("refunded_amount")]
        public decimal RefundedAmount { get; set; } = 0;

        [JsonPropertyName("NOC_auto_corrected_at")]
        public DateTime? NocAutoCorrectedAt { get; set; }
    }
}
