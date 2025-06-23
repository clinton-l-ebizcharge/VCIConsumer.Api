using System.Net;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Extensions;

public static IResult ToErrorResponse(this Exception ex, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
{
    var response = new ApiResponse
    {
        IsSuccess = false,
        StatusCode = statusCode,
        Result = new object(),
        Errors = new List<ErrorResponse>
        {
            new ErrorResponse
            {
                Code = "Error",
                Message = ex.Message,
                Type = ex.GetType().Name
            }
        }
    };

    return Results.Json(response, statusCode: (int)statusCode);
}
