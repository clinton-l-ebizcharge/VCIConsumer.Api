using VCIConsumer.Api.Models.Responses;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace VCIConsumer.Api.Extensions;

public static class ApiResultHandlerExtensions
{
    public static IResult ToApiResult<T>(this T? result, string notFoundMessage = "Resource not found")
    {
        if (result is null || (result is ICollection<object> col && col.Count == 0))
            return ApiResponseExtensions.ToNotFoundResponse(notFoundMessage);

        return result.ToSuccessResponse();
    }
}
