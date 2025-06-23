using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses;

public class PaymentBankAccountResponse : BaseBankAccountResponse
{
    [JsonPropertyName("uuid")]
    public string UUId { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
}

