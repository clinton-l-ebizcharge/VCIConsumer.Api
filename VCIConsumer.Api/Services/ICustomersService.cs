using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Requests;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Services;
public interface ICustomersService
{        
    Task<IResult> CustomerListAsync();
    Task<IResult> CustomerDetailAsync(string customer_uuid);
    Task<IResult> CustomerCreationAsync(CustomerCreationRequest request);
    Task<IResult> CustomerUpdateAsync(CustomerUpdateRequest request);    
}