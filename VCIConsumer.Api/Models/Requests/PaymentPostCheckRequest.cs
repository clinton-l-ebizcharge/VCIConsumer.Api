using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Requests;

public class PaymentPostCheckRequest
{
    [JsonPropertyName("check_number")]
    public string CheckNumber { get; set; } = string.Empty;

    [JsonPropertyName("check_image_front")]
    public string CheckImageFront { get; set; } = string.Empty; // base64 string

    [JsonPropertyName("check_image_back")]
    public string CheckImageBack { get; set; } = string.Empty; // base64 string
}
