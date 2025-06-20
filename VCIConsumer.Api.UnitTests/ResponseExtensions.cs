using Microsoft.AspNetCore.Http;

namespace VCIConsumer.Api.Extensions;
// Dummy extension methods to simulate your existing ones in VCIConsumer.Api.Extensions.
// In your project these are defined separately.
public static class ResponseExtensions
{
    public static IResult ToSuccessResponse(this object obj)
    {
        // For testing, wrap the object in an ok result.
        return Results.Ok(new { Success = true, Data = obj });
    }
    public static IResult ToFailureResponse(this string error, string title)
    {
        // For testing, return a ProblemDetails result.
        return Results.Problem(detail: error, title: title);
    }
    public static IResult ToValidationFailure(this string error)
    {
        // For testing, return a BadRequest result.
        return Results.BadRequest(new { Error = error });
    }
}
