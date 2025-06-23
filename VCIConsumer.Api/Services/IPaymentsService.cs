using VCIConsumer.Api.Models.Query;
using VCIConsumer.Api.Models.Requests;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Services;
public interface IPaymentsService
{
    Task<List<PaymentListResponse>?> PaymentListAsync(PaymentListQuery query);
    Task<PaymentDetailResponse> PaymentDetailAsync(string paymentUuId);
    Task<PaymentPostResponse> PaymentPostAsync(PaymentPostRequest request);
    Task<PaymentPostWithTokenResponse> PaymentPostWithTokenAsync(string customer_uuid, PaymentPostWithTokenRequest request);
    Task<PaymentUpdateResponse> PaymentUpdateAsync(string paymentUuId, PaymentUpdateRequest request);
}
