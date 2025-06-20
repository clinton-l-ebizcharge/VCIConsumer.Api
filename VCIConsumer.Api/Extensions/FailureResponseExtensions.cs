using System.Net;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Extensions;

public static class FailureResponseExtensions
{
    public static IResult ToValidationFailure(
        this string errorMessage,
        string errorCode = "VALIDATION_FAILED")
    {
        return errorMessage.ToFailureResponse(errorCode, HttpStatusCode.BadRequest);
    }

    public static IResult ToNotFoundResponse(
        this string errorMessage,
        string errorCode = "NOT_FOUND")
    {
        return errorMessage.ToFailureResponse(errorCode, HttpStatusCode.NotFound);
    }

    public static IResult ToInternalErrorResponse(
        this string errorMessage,
        string errorCode = "SERVER_ERROR")
    {
        return errorMessage.ToFailureResponse(errorCode, HttpStatusCode.InternalServerError);
    }
}
