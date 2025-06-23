namespace VCIConsumer.Api.Models.Responses;

using System.Text.Json.Serialization;

public class PaymentCustomerResponse
{
    [JsonPropertyName("email_truncated")]
    public string EmailTruncated { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}


