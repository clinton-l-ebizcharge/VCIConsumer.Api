using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses;

public class WarningBankAccount
{
    [JsonPropertyName("account_number_truncated")]
    public string AccountNumberTruncated { get; set; }

    [JsonPropertyName("route_number_truncated")]
    public string RouteNumberTruncated { get; set; }

    [JsonPropertyName("account_type")]
    public string AccountType { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
}

