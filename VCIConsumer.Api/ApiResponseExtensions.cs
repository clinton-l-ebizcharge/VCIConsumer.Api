using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Responses;
using System.Net;

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
    /// Wraps an Exception into an error ApiResponse and produces a custom status code result.    
    /// </summary>    
    public static IResult ToErrorResponse(this Exception ex, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        var response = new ApiResponse
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Result = new object(), 
            Errors = new List<ErrorResponse>
            {
                new ErrorResponse { Code = "Error", Message = ex.Message, Type = "Exception" } // Ensure all required members of ErrorResponse are set    
            }
        };

        return Results.Json(response, statusCode: (int)statusCode);
    }
}

