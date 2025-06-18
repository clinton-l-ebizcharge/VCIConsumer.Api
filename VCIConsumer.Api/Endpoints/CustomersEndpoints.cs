using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using VCIConsumer.Api.Models.Requests;
using VCIConsumer.Api.Models.Responses;
using VCIConsumer.Api.Services;

namespace VCIConsumer.Api.Endpoints;

// Removed inheritance from EndpointsBase as static classes cannot inherit from any type.
public static class CustomersEndpoints
{
    private const string ServiceName = "customers";

    [Tags("Customers Endpoints")]
    public static void ConfigureCustomersEndpoints(this WebApplication app)
    {
        app.MapGet(
            "/customers", CustomerList)
            .WithName("Customer List")
            .Accepts<IResult>("application/json")
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse>(StatusCodes.Status401Unauthorized)            
            .Produces<ApiResponse>(StatusCodes.Status500InternalServerError);

        app.MapGet(
            "/customers/{customer_uuid}", CustomerDetail)
            .WithName("Customer Detail")
            .Accepts<IResult>("application/json")
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse>(StatusCodes.Status500InternalServerError);

        app.MapPost(
            "/customers", CustomerCreation)
            .WithName("Customer Creation")
            .Accepts<IResult>("application/json")
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse>(StatusCodes.Status500InternalServerError);

        app.MapPatch("/customers", CustomerList)
            .WithName("Customer Update")
            .Accepts<IResult>("application/json")
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse>(StatusCodes.Status500InternalServerError);
    }

    [Tags("Customers")]
    private static async Task<IResult> CustomerList(
        [FromServices] ICustomersService customersservice,
        [FromServices] IHttpClientFactory httpClientFactory,
        string? sort = null,
        int limitPerPage = 100,
        int pageNumber = 1)
    {        
        var response = await customersservice.CustomerListAsync();
        return Results.Ok("CustomerList");
    }

    [Tags("Customers")]
    private static async Task<IResult> CustomerDetail(
        [FromServices] ICustomersService customersservice, 
        [FromServices] IHttpClientFactory httpClientFactory, 
        string customer_uuid)
    {
        var response = await customersservice.CustomerDetailAsync(customer_uuid);
        return Results.Ok("Customer Detail");
    }

    [Tags("Customers")]
    private static async Task<IResult> CustomerCreation(
        [FromServices] ICustomersService customersservice, 
        [FromServices] IHttpClientFactory httpClientFactory, 
        [FromBody] CustomerCreationRequest request)
    {
        var response = await customersservice.CustomerCreationAsync(request);
       return Results.Ok("Customer Creation");
    }

    [Tags("Customers")]
    private static async Task<IResult> CustomerUpdate(
        [FromServices] ICustomersService customersservice, 
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromBody] CustomerUpdateRequest request)
    {
        var response = await customersservice.CustomerUpdateAsync(request);
        return Results.Ok("CustomerUpdate");
    }

    private static string GetDebuggerDisplay(this Program program) => $"{program.GetType().Name}";
}
