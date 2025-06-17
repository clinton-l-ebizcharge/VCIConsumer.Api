namespace VCIConsumer.Api.Models;

public class _RefundResult : _PaymentResult
{
    public required string original_payment_uuid { get; set; }
}
