using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses;

public class PaymentPostCustomerResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("uuid")]
    public string UUId { get; set; }
    [JsonPropertyName("bank_account")]
    public PaymentPostCustomerBankAccountResponse BankAccount { get; set; }
    [JsonPropertyName("active")]
    public bool Active { get; set; } 
}

