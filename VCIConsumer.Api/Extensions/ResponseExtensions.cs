using System.Net;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Extensions;

public static class ResponseExtensions
{
    // Success response (already added)
    public static IResult ToSuccessResponse<T>(this T result, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var apiResponse = new ApiResponse
        {
            IsSuccess = true,
            StatusCode = statusCode,
            Result = result!, // Fix: Ensure 'Result' is set to avoid CS9035
            Errors = new List<ErrorResponse>() // Fix: Initialize 'Errors' to avoid null reference issues
        };

        return Results.Ok(apiResponse);
    }

    // Failure response with optional error code
    public static IResult ToFailureResponse(this string errorMessage, string? errorCode = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        var error = new ErrorResponse
        {
            Code = errorCode ?? "GENERIC_ERROR",
            Message = errorMessage,
            Type = "Failure" // Fix: Set the required 'Type' property
        };

        var apiResponse = new ApiResponse
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Result = new object(), // Fix: Provide a default value for 'Result' to avoid CS9035
            Errors = new List<ErrorResponse> { error } // Fix: Correct syntax for initializing Errors list
        };

        return Results.Json(apiResponse, statusCode: (int)statusCode); // Fix: Cast HttpStatusCode to int
    }

    // Problem response with optional developer log
    public static IResult ToProblemResponse(this Exception ex, string? developerNote = null, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        var error = new ErrorResponse
        {
            Code = ex.GetType().Name,
            Message = ex.Message,
            Type = "Exception" // Fix: Set the required 'Type' property
        };

        var apiResponse = new ApiResponse
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Result = new object(), // Fix: Provide a default value for 'Result' to avoid CS9035
            Errors = new List<ErrorResponse> { error } // Fix: Correct syntax for initializing Errors list
        };

        if (!string.IsNullOrWhiteSpace(developerNote))
        {
            apiResponse.Errors.Add(new ErrorResponse
            {
                Message = developerNote,
                Code = "DeveloperNote",
                Type = "Note" // Fix: Set the required 'Type' property
            });
        }

        return Results.Json(apiResponse, statusCode: (int)statusCode); // Fix: Cast HttpStatusCode to int
    }
}

