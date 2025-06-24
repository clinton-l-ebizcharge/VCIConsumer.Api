using System.Text.Json.Serialization;
using static VCIConsumer.Api.Models.Enums.Enums;

namespace VCIConsumer.Api.Models.Responses;

public class PaymentPostCustomerBankAccountResponse
{
    [JsonPropertyName("account_number_truncated")]
    public required string AccountNumberTruncated { get; set; }
    [JsonPropertyName("route_number_truncated")]
    public required string RoutingNumberTruncated { get; set; }
    [JsonPropertyName("account_type")]
    public required PaymentPostAccountTypes AccountType { get; set; } // Checking or Savings
}
