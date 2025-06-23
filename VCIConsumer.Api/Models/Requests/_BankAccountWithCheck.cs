using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Requests;

public class _BankAccountWithCheck
{
    [JsonPropertyName("check")]
    public required PaymentCheck Check { get; set; }
}
