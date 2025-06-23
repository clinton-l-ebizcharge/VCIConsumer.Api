using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses;

public class BaseBankAccountResponse
{
    [JsonPropertyName("account_number_truncated")]
    public string AccountNumberTruncated { get; set; }

    [JsonPropertyName("routing_number_truncated")]
    public string RoutingNumberTruncated { get; set; }

    [JsonPropertyName("account_type")]
    public string AccountType { get; set; }
}

