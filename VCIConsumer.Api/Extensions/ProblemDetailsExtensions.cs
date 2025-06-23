using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace VCIConsumer.Api.Extensions;

public static class ProblemDetailsExtensions
{
    public static ProblemDetails ToProblemDetails(this Exception ex, HttpStatusCode statusCode, bool includeDetails = false)
    {
        var problem = new ProblemDetails
        {
            Title = "An error occurred while processing your request.",
            Status = (int)statusCode,
            Detail = ex.Message,
            Type = $"https://httpstatuses.com/{(int)statusCode}"
        };

        if (includeDetails)
        {
            problem.Extensions["exception"] = ex.GetType().Name;
            problem.Extensions["stackTrace"] = ex.StackTrace;

            if (ex.InnerException is not null)
            {
                problem.Extensions["innerException"] = new
                {
                    type = ex.InnerException.GetType().Name,
                    message = ex.InnerException.Message
                };
            }
        }

        return problem;
    }

    public static IResult ToProblemResult(this Exception ex, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, bool includeDetails = false)
    {
        var problem = ex.ToProblemDetails(statusCode, includeDetails);
        return Results.Problem(
            detail: problem.Detail,
            statusCode: problem.Status,
            title: problem.Title,
            type: problem.Type,
            extensions: problem.Extensions
        );
    }
}

