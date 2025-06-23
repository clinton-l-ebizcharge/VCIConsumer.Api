using System.Text.Json.Serialization;
using static VCIConsumer.Api.Models.Enums.Enums;

namespace VCIConsumer.Api.Models.Requests;

public class PaymentPostWithTokenRequest
{
    [JsonPropertyName("amount")]
    public required double Amount { get; set; }
    [JsonPropertyName("standard_entry_class")]
    public required StandardEntryClass StandardEntryClass { get; set; }
    [JsonPropertyName("customer")]
    public required PaymentPostWithTokenCustomerRequest Customer { get; set; }
    [JsonPropertyName("description")]
    public required string Description { get; set; }
    [JsonPropertyName("addenda")]
    public string Addenda { get; set; } = string.Empty;
    [JsonPropertyName("check")]
    public PaymentPostWithTokenCheckRequest? Check { get; set; }
}
