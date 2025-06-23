using System.Text.Json.Serialization;
using static VCIConsumer.Api.Models.Enums.Enums;

namespace VCIConsumer.Api.Models.Requests;

public class PaymentPostRequest
{
    [JsonPropertyName("amount")]
    public required double Amount { get; set; }
    [JsonPropertyName("standard_entry_class")]
    public required StandardEntryClass StandardEntryClass { get; set; }
    [JsonPropertyName("description")]
    public required string Description { get; set; }
    [JsonPropertyName("addenda")]
    public string Addenda { get; set; } = string.Empty;
    [JsonPropertyName("customer")]
    public required PaymentPostCustomerRequest Customer { get; set; }
    [JsonPropertyName("bank_account")]
    public PaymentPostBankAccountRequest? BankAccount { get; set; }
    [JsonPropertyName("check")]
    public PaymentPostCheckRequest? Check { get; set; }
}
