using FluentValidation.Results;
using System.Net;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Extensions;

public static class ApiResponseExtensions
{
    /// <summary>
    /// Wraps the data into a successful ApiResponse and produces an Ok result.
    /// </summary>
    public static IResult ToSuccessResponse<T>(this T result)
    {
        var response = new ApiResponse
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            Result = result ?? new object(),
            Errors = new List<ErrorResponse>()
        };

        return Results.Ok(response);
    }

    /// <summary>
    /// Wraps an exception into an error ApiResponse and produces a custom status code result.
    /// </summary>
    public static List<ErrorResponse> ToErrorResponses(this ValidationResult result)
    {
        return result.Errors.Select(f => new ErrorResponse
        {
            Code = "Validation",
            Message = f.ErrorMessage,
            Type = "Validation"
        }).ToList();
    }

    /// <summary>
    /// Produces a standardized 404 Not Found response with a message.
    /// </summary>
    public static IResult ToNotFoundResponse(string message = "Resource not found")
    {
        var response = new ApiResponse
        {
            IsSuccess = false,
            StatusCode = HttpStatusCode.NotFound,
            Result = new object(),
            Errors = new List<ErrorResponse>
            {
                new ErrorResponse { Code = "NotFound", Message = message, Type = "Client" }
            }
        };

        return Results.NotFound(response);
    }

    /// <summary>
    /// Produces a validation error response with details.
    /// </summary>
    public static IResult ToValidationErrorResponse(IEnumerable<ErrorResponse> errors)
    {
        var response = new ApiResponse
        {
            IsSuccess = false,
            StatusCode = HttpStatusCode.BadRequest,
            Result = new object(),
            Errors = errors.ToList()
        };

        return Results.BadRequest(response);
    }

    public static IResult ToErrorResponse(this Exception ex, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, bool includeDetails = false)
    {
        var errors = new List<ErrorResponse>
    {
        new ErrorResponse
        {
            Code = "Error",
            Message = ex.Message,
            Type = ex.GetType().Name
        }
    };

        if (includeDetails)
        {
            if (ex.InnerException is not null)
            {
                errors.Add(new ErrorResponse
                {
                    Code = "InnerException",
                    Message = ex.InnerException.Message,
                    Type = ex.InnerException.GetType().Name
                });
            }

            errors.Add(new ErrorResponse
            {
                Code = "StackTrace",
                Message = ex.StackTrace ?? "No stack trace available",
                Type = "Diagnostic"
            });
        }

        var response = new ApiResponse
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Result = new object(),
            Errors = errors
        };

        return Results.Json(response, statusCode: (int)statusCode);
    }


}
