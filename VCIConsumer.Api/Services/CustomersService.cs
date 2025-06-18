using Microsoft.Extensions.Options;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Requests;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Services;

public class CustomersService(IOptions<ApiSettings> apiSettings, IHttpClientFactory httpClientFactory, TokenService tokenService)
    : ServiceBase(apiSettings, httpClientFactory, tokenService), ICustomersService
{
    private readonly ApiSettings _apiSettings = apiSettings.Value;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<IResult> CustomerList()
    {
        var apiResponse = await APIGet<CustomerResult[]>($"customers");
        //if (rs.HasError)
        //    return Results.Problem(detail: rs.Errors[0].ToString());

        //if (rs.Data == null || rs.Data.Length == 0)
        //    return Results.NotFound("No customers found.");

        return Results.Ok(apiResponse);
    }

    public async Task<IResult> CustomerDetail(string customer_uuid)
    {
        customer_uuid = customer_uuid.StartsWith("CUS_", StringComparison.OrdinalIgnoreCase) ? customer_uuid : $"CUS_{customer_uuid}";
        var response = await APIGet<CustomerDetailResponse>($"customers/{customer_uuid}");
        return Results.Ok(response);
    }

    public async Task<IResult> CustomerCreation(CustomerCreationRequest request)
    {
        var content = CreateHttpContent(request);
        var apiResponse = await APIPost<CustomerResponse>("customers", content);

        // Fix: Access the error message directly as a string instead of trying to access a non-existent 'code' property.
        //if (!apiResponse.IsSuccess && apiResponse.Errors.Count > 0 && apiResponse.Errors[0].Contains("CU028"))
        //{
        //    return Results.Problem(detail: apiResponse.Errors[0]);
        //}

        return Results.Ok(apiResponse);
    }

    public async Task<IResult> CustomerUpdate(CustomerUpdateRequest request)
    {  
        var response = await APIPatch<CustomerResponse>("customers", CreateHttpContent(request));
        return Results.Ok(response);
    }
}