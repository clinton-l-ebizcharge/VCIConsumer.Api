using System.Text.Json.Serialization;
using static VCIConsumer.Api.Models.Enums.Enums;

namespace VCIConsumer.Api.Models.Requests;

public class PaymentPostBankAccountRequest
{
    [JsonPropertyName("routing_number")]
    public required string RoutingNumber { get; set; }

    [JsonPropertyName("account_number")]
    public required string AccountNumber { get; set; }

    [JsonPropertyName("account_type")]
    public required PaymentPostAccountTypes AccountType { get; set; } // Checking or Savings
}
