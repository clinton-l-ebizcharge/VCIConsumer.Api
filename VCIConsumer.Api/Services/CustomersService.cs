using Microsoft.Extensions.Options;
using System.Net.Http;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Requests;
using VCIConsumer.Api.Models.Responses;
using VCIConsumer.Api.Extensions;

namespace VCIConsumer.Api.Services;

public class CustomersService(IOptions<ApiSettings> apiSettings, IHttpClientFactory httpClientFactory) : ICustomersService
{
    private readonly ApiSettings _apiSettings = apiSettings.Value;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<IResult> CustomerListAsync()
    {
        var client = _httpClientFactory.CreateClient("VCIApi");
        var response = await client.GetAsync("customers");

        response.EnsureSuccessStatusCode();
        return Results.Ok(response);
    }

    public async Task<IResult> CustomerDetailAsync(string customer_uuid)
    {
        customer_uuid = customer_uuid.StartsWith("CUS_", StringComparison.OrdinalIgnoreCase) ? customer_uuid : $"CUS_{customer_uuid}";
        var client = _httpClientFactory.CreateClient("VCIApi");
        var response = await client.GetAsync($"customers/{customer_uuid}");
        return Results.Ok(response);
    }

    public async Task<IResult> CustomerCreationAsync(CustomerCreationRequest request)
    {        
        var client = _httpClientFactory.CreateClient("VCIApi");
        var httpContent = JsonContent.Create(request);
        var response = await client.PostAsync("customers", httpContent);

        // Fix: Access the error message directly as a string instead of trying to access a non-existent 'code' property.
        //if (!apiResponse.IsSuccess && apiResponse.Errors.Count > 0 && apiResponse.Errors[0].Contains("CU028"))
        //{
        //    return Results.Problem(detail: apiResponse.Errors[0]);
        //}

        return Results.Ok(response);
    }

    public async Task<IResult> CustomerUpdateAsync(CustomerUpdateRequest request)
    {
        var client = _httpClientFactory.CreateClient("VCIApi");
        var httpContent = JsonContent.Create(request);
        var response = await client.PatchAsync("customers", httpContent);
        return Results.Ok(response);
    }
}