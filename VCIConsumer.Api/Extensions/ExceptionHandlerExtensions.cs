using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace VCIConsumer.Api.Extensions;

public static class ExceptionHandlerExtensions
{
    public static IApplicationBuilder UseStandardApiExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appBuilder =>
        {
            appBuilder.Run(async context =>
            {
                var env = context.RequestServices.GetRequiredService<IHostEnvironment>();
                var logger = context.RequestServices.GetRequiredService<ILogger<Exception>>();
                var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionFeature?.Error;

                var statusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                if (exception is not null)
                {
                    logger.LogError(exception, "Unhandled exception caught by global handler.");

                    var result = new
                    {
                        isSuccess = false,
                        statusCode,
                        errors = new[]
                        {
                            new { code = "Exception", message = exception.Message, type = "Error" }
                        }
                    };

                    await context.Response.WriteAsJsonAsync(result, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                }
                else
                {
                    logger.LogWarning("Unhandled exception occurred with no exception detail.");

                    await context.Response.WriteAsJsonAsync(new
                    {
                        isSuccess = false,
                        statusCode,
                        errors = new[]
                        {
                            new { code = "Unhandled", message = "An unexpected error occurred.", type = "Fallback" }
                        }
                    });
                }
            });
        });

        return app;
    }
}

