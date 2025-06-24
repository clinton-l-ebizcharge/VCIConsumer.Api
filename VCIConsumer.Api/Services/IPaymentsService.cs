using VCIConsumer.Api.Models.Query;
using VCIConsumer.Api.Models.Requests;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Services;
public interface IPaymentsService
{
    Task<PaymentListResponse?> PaymentListAsync(PaymentListQuery query);
    Task<PaymentDetailResponse> PaymentDetailAsync(string payment_uuid);
    Task<PaymentPostResponse> PaymentPostAsync(PaymentPostRequest request);
    Task<PaymentPostWithTokenResponse> PaymentPostWithTokenAsync(string customer_uuid, PaymentPostWithTokenRequest request);
    Task<PaymentUpdateResponse> PaymentUpdateAsync(string payment_uuid, PaymentUpdateRequest request);
}
