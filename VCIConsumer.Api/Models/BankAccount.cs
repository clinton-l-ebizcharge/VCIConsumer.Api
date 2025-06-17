using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models;

public class BankAccount
{
    [JsonPropertyName("routing_number_truncated")]
    public required string routing_number { get; set; }
    [JsonPropertyName("account_number_truncated")]
    public required string account_number { get; set; }
    [JsonPropertyName("account_type")]
    public required string AccountType { get; set; }
    [JsonPropertyName("account_validated")]
    public bool AccountValidated { get; set; }
}
