using System.Text.Json.Serialization;
using static VCIConsumer.Api.Models.Enums.Enums;

namespace VCIConsumer.Api.Models;

public class BankAccountRequest
{
    [JsonPropertyName("routing_number")]
    public string RoutingNumber { get; set; }
    [JsonPropertyName("account_number")]
    public string AccountNumber { get; set; }
    [JsonPropertyName("account_type")]
    public AccountTypes AccountType { get; set; }
}
