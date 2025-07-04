﻿using FluentValidation;
using System.Net;
using VCIConsumer.Api.Extensions;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Filter;

public class StandardValidatedApiFilter<T> : IEndpointFilter
{
    private readonly IValidator<T> _validator;

    public StandardValidatedApiFilter(IValidator<T> validator) => _validator = validator;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var model = context.Arguments.OfType<T>().FirstOrDefault();
        if (model is not null)
        {
            var result = await _validator.ValidateAsync(model);
            if (!result.IsValid)
            {
                var errors = result.Errors.Select(e => new ErrorResponse
                {
                    Code = "VALIDATION",
                    Message = e.ErrorMessage,
                    Type = "Validation"
                });

                // Ensure the error message passed to ToFailureResponse is not null
                var errorMessage = "Validation failed";
                return new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    Errors = errors.ToList(),                    
                    Timestamp = DateTime.UtcNow
                }.ToString().ToFailureResponse(errorMessage ?? "Unknown error");
            }
        }

        var response = await next(context);
        return response is IResult ? response : ((object)response!).ToSuccessResponse();
    }
}

