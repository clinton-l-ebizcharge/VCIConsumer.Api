using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Requests;

public class PaymentUpdateRequest
{
    [JsonPropertyName("status")]
    public string Status { get; set; } //void
}
