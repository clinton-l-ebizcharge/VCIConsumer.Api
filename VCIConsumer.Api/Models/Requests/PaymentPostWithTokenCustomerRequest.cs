using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Requests;

public class PaymentPostWithTokenCustomerRequest
{
    [JsonPropertyName("uuid")]
    public required string UUId { get; set; }
}
