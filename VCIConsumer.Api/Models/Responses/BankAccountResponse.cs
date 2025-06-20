using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses;

public class BankAccountResponse
{
    [JsonPropertyName("account_type")]
    public string AccountType { get; set; }

    [JsonPropertyName("account_validated")]
    public bool AccountValidated { get; set; }

    [JsonPropertyName("account_number_truncated")]
    public string AccountNumberTruncated { get; set; }

    [JsonPropertyName("routing_number_truncated")]
    public string RoutingNumberTruncated { get; set; }
}
