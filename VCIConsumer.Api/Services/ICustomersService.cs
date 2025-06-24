using VCIConsumer.Api.Models.Query;
using VCIConsumer.Api.Models.Requests;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Services;
public interface ICustomersService
{
    Task<CustomerListResponse?> CustomerListAsync(CustomerListQuery query);
    Task<CustomerDetailResponse> CustomerDetailAsync(string customerUuid);
    Task<CustomerCreationResponse> CustomerCreationAsync(CustomerCreationRequest request);
    Task<CustomerUpdateResponse> CustomerUpdateAsync(CustomerUpdateRequest request);
}
