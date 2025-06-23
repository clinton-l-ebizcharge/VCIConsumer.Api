using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses;

public class PaymentApiErrorEnvelope
{
    [JsonPropertyName("errors")]
    public List<ErrorResponse> Errors { get; set; }

    [JsonPropertyName("is_return_rectified")]
    public bool IsReturnRectified { get; set; }
}
