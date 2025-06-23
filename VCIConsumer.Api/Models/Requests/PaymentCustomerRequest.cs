using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Requests;

public class PaymentPostCustomerRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
}
