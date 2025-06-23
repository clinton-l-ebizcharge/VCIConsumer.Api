using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using VCIConsumer.Api.Extensions;
using VCIConsumer.Api.Filter;
using VCIConsumer.Api.Models.Query;
using VCIConsumer.Api.Models.Requests;
using VCIConsumer.Api.Models.Responses;
using VCIConsumer.Api.Services;

namespace VCIConsumer.Api.Endpoints;

// Removed inheritance from EndpointsBase as static classes cannot inherit from any type.
public static class CustomersEndpoints
{
    public static void MapCustomersEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/customers")
               .WithTags("Customers")
               .WithOpenApi();                              

        group.MapGet(
            "/", CustomerListAsync)
            .WithName("CustomerList")
            .WithOpenApi()
            .WithStandardApiResponses()
            .Produces<ApiResponse>(StatusCodes.Status404NotFound, "application/json")
            .AddEndpointFilter<StandardApiFilter<List<CustomerListResponse>>>()
            .AddEndpointFilter<StandardValidatedApiFilter<CustomerListQuery>>();

        group.MapGet(
            "/{customer_uuid}", CustomerDetailAsync)
            .WithName("CustomerDetail")
            .WithStandardApiResponses()
            .Produces<ApiResponse>(StatusCodes.Status404NotFound, "application/json")
            .AddEndpointFilter<StandardApiFilter<List<CustomerDetailResponse>>>()
            .AddSimpleInputValidation<string>(uuid =>
            {
                if (string.IsNullOrWhiteSpace(uuid))
                    return (false, "Customer UUID must not be empty.");

                var normalizedUuid = uuid.StartsWith("CUS_", StringComparison.OrdinalIgnoreCase)
                    ? uuid
                    : $"CUS_{uuid}";

                if (uuid.Length < 8)
                    return (false, "Customer UUID is too short.");

                return (true, null);
            });

        group.MapPost("/", CustomerCreationAsync)
            .WithName("CustomerCreation")
            .Accepts<CustomerCreationRequest>("application/json")
            .WithStandardApiResponses()
            .AddEndpointFilter<StandardApiFilter<CustomerCreationResponse>>(); 

        group.MapPatch("/", CustomerUpdateAsync)
            .WithName("CustomerUpdate")
            .Accepts<CustomerCreationRequest>("application/json")
            .WithStandardApiResponses()
            .AddEndpointFilter<StandardApiFilter<CustomerCreationResponse>>();
    }

    private static async Task<IResult> CustomerListAsync(
        [AsParameters] CustomerListQuery query,
        [FromServices] ICustomersService customersservice)
    {        
        var response = await customersservice.CustomerListAsync(query);
        return response.ToSuccessResponse();
    }

    private static async Task<IResult> CustomerDetailAsync(
        [FromRoute] string customer_uuid,
        [FromServices] ICustomersService customersservice, 
        [FromServices] IHttpClientFactory httpClientFactory)
    {
        var response = await customersservice.CustomerDetailAsync(customer_uuid);
        return response.ToSuccessResponse();
    }

    private static async Task<IResult> CustomerCreationAsync(
        [FromServices] ICustomersService customersservice, 
        [FromServices] IHttpClientFactory httpClientFactory, 
        [FromBody] CustomerCreationRequest request)
    {
       var response = await customersservice.CustomerCreationAsync(request);
       return response.ToSuccessResponse();
    }

    private static async Task<IResult> CustomerUpdateAsync(
        [FromServices] ICustomersService customersservice, 
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromBody] CustomerUpdateRequest request)
    {
        var response = await customersservice.CustomerUpdateAsync(request);
        return response.ToSuccessResponse();
    }

    private static string GetDebuggerDisplay(this Program program) => $"{program.GetType().Name}";
}
