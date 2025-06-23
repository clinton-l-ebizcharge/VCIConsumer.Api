using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using VCIConsumer.Api.Extensions;
using VCIConsumer.Api.Filter;
using VCIConsumer.Api.Models.Query;
using VCIConsumer.Api.Models.Requests;
using VCIConsumer.Api.Models.Responses;
using VCIConsumer.Api.Services;

namespace VCIConsumer.Api.Endpoints;

// Removed inheritance from EndpointsBase as static classes cannot inherit from any type.
public static class PaymentsEndpoints
{
    public static void MapPaymentsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/payments")
               .WithTags("Payments")
               .WithOpenApi();

        group.MapGet(
            "/", PaymentListAsync)
            .WithName("PaymentList")
            .WithOpenApi()
            .WithStandardApiResponses()
            .Produces<ApiResponse>(StatusCodes.Status404NotFound, "application/json")
            .AddEndpointFilter<StandardApiFilter<List<PaymentListResponse>>>()
            .AddEndpointFilter<StandardValidatedApiFilter<PaymentListQuery>>();

        group.MapGet(
            "/{payment_uuid}", PaymentDetailAsync)
            .WithName("PaymentDetail")
            .WithStandardApiResponses()
            .Produces<ApiResponse>(StatusCodes.Status404NotFound, "application/json")
            .AddEndpointFilter<StandardApiFilter<List<PaymentDetailResponse>>>()
            .AddSimpleInputValidation<string>(uuid =>
            {
                if (string.IsNullOrWhiteSpace(uuid))
                    return (false, "Payment UUID must not be empty.");

                var normalizedUuid = uuid.StartsWith("CUS_", StringComparison.OrdinalIgnoreCase)
                    ? uuid
                    : $"CUS_{uuid}";

                if (uuid.Length < 8)
                    return (false, "Payment UUID is too short.");

                return (true, null);
            });

        group.MapPost("/", PaymentPostAsync)
            .WithName("PaymentPost")
            .Accepts<PaymentPostRequest>("application/json")
            .WithStandardApiResponses()
            .WithOpenApi()
            .AddEndpointFilter<StandardApiFilter<PaymentPostResponse>>();

        group.MapPost("/{customer_uuid}", PaymentPostWithTokenAsync)
            .WithName("PaymentPostWithToken")
            .Accepts<PaymentPostWithTokenRequest>("application/json")
            .WithStandardApiResponses()
            .AddEndpointFilter<StandardApiFilter<PaymentPostWithTokenResponse>>();

        group.MapPatch("/{payment_uuid}", PaymentUpdateAsync)
            .WithName("PaymentUpdate")
            .Accepts<PaymentUpdateRequest>("application/json")
            .WithStandardApiResponses()
            .AddEndpointFilter<StandardApiFilter<PaymentUpdateResponse>>();
    }

    private static async Task<IResult> PaymentListAsync(
        [AsParameters] PaymentListQuery query,
        [FromServices] IPaymentsService paymentsService)
    {
        var response = await paymentsService.PaymentListAsync(query);
        return response.ToApiResult("No payments found");
    }

    private static async Task<IResult> PaymentDetailAsync(
        [FromRoute] string payment_uuid,
        [FromServices] IPaymentsService paymentsService)
    {
        try
        {
            var result = await paymentsService.PaymentDetailAsync(payment_uuid);
            return result.ToApiResult("Payment not found");
        }
        catch (Exception ex)
        {
            return ex.ToProblemResult(HttpStatusCode.InternalServerError);
        }
    }


    private static async Task<IResult> PaymentPostAsync(
        [FromServices] IPaymentsService paymentsService,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromBody] PaymentPostRequest request)
    {
        try
        {
            var result = await paymentsService.PaymentPostAsync(request);
            return result.ToApiResult();
        }
        catch (Exception ex)
        {
            return ex.ToProblemResult(HttpStatusCode.InternalServerError);
        }                
    }

    private static async Task<IResult> PaymentPostWithTokenAsync(
        [FromServices] IPaymentsService paymentsService,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromRoute] string customer_uuid,
        [FromBody] PaymentPostWithTokenRequest request)
    {

        var response = await paymentsService.PaymentPostWithTokenAsync(customer_uuid, request);
        return response.ToApiResult();
    }

    private static async Task<IResult> PaymentUpdateAsync(
        [FromServices] IPaymentsService paymentsService,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromRoute] string paymentUuId,
        [FromBody] PaymentUpdateRequest request)
    {
        var response = await paymentsService.PaymentUpdateAsync(paymentUuId, request);
        return response.ToApiResult();
    }

    private static string GetDebuggerDisplay(this Program program) => $"{program.GetType().Name}";
}
