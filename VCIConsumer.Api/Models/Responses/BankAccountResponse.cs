using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses;

public class BankAccountResponse
{
    [JsonPropertyName("account_type")]
    public required string AccountType { get; set; }
    [JsonPropertyName("account_validated")]
    public bool AccountValidated { get; set; }  
    [JsonPropertyName("account_number_truncated")]
    public required string AccountNumber { get; set; }
    [JsonPropertyName("routing_number_truncated")]
    public required string RoutingNumber { get; set; }
}
