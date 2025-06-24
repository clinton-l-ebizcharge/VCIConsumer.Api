using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses;

public class PaymentListResponse
{
    [JsonPropertyName("customers")]
    public List<PaymentListPaymentDetailResponse>? Payments { get; set; }
}
