using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Requests;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Services;
public interface ICustomersService
{        
    Task<IResult> CustomerList();
    Task<IResult> CustomerDetail(string customer_uuid);
    Task<IResult> CustomerCreation(CustomerCreationRequest request);
    Task<IResult> CustomerUpdate(CustomerUpdateRequest request);    
}