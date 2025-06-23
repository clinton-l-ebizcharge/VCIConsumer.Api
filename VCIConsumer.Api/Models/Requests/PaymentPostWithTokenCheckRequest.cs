using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Requests;

public class PaymentPostWithTokenCheckRequest
{
    [JsonPropertyName("check_number")]
    public required string CheckNumber { get; set; }

    [JsonPropertyName("check_image_front")]
    public required string CheckImageFront { get; set; }

    [JsonPropertyName("check_image_back")]
    public required string CheckImageBack { get; set; }
}
