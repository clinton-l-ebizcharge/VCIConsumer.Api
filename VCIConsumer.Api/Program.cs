using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Json;
using Scalar.AspNetCore;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Endpoints;
using VCIConsumer.Api.Filter;
using VCIConsumer.Api.Handler;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Responses;
using VCIConsumer.Api.Services;

namespace VCIConsumer.Api;

public partial class Program {
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHttpContextAccessor();

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            options.SerializerOptions.PropertyNamingPolicy = null;
            options.SerializerOptions.WriteIndented = false;
            options.SerializerOptions.IncludeFields = false;
        });

        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<CustomerQueryValidator>();
        builder.Services.AddScoped(typeof(StandardValidatedApiFilter<>));

        builder.Services.AddScoped<ICustomersService, CustomersService>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddSingleton<ITokenService, TokenService>();

        //builder.Services.AddSingleton<IEndpointFilterFactory, StandardApiFilterFactory>();
        
        builder.Services.AddAntiforgery();

        builder.Services.Configure<LoggingOptions>(builder.Configuration.GetSection("LoggingOptions"));

        builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));        
        var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var apiClientName = configurationBuilder.Build().GetValue<string>("ApiSettings:ApiClientName") ?? "VCIApi";

        builder.Services.AddHttpContextAccessor();
        
        //Handlers as transients
        builder.Services.AddTransient<TokenHandler>();
        builder.Services.AddTransient<CorrelationLoggingHandler>();

        builder.Services.AddHttpClient(apiClientName, client =>
        {
            var baseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://api.sandbox.vericheck.com";
            client.BaseAddress = new Uri(baseUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));            
            var vericheckVersion = builder.Configuration.GetValue<string>("ApiSettings:VericheckVersion") ?? "1.13";
            client.DefaultRequestHeaders.Add("VeriCheck-Version", vericheckVersion);            
        })
        .AddHttpMessageHandler<TokenHandler>()        
        .AddHttpMessageHandler<CorrelationLoggingHandler>();

        builder.Services.AddAuthentication();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapOpenApi();

            app.MapScalarApiReference(options =>
            {
                options.Title = "VCI (Vericheck) Api Consumer";
                options.Layout = ScalarLayout.Modern; // Set default layout
                options.ShowSidebar = true; // Ensure sidebar is visible
                //options.WithForceThemeMode(ThemeMode.Dark); // Default to dark mode if preferred
                options.WithOpenApiRoutePattern("/openapi/v1.json");
                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.AsyncHttp);
            });
        };

        app.UseHttpsRedirection();

        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async httpContext =>
            {
                var problemDetailService = httpContext.RequestServices.GetService<IProblemDetailsService>();
                if (problemDetailService == null
                    || !await problemDetailService.TryWriteAsync(new() { HttpContext = httpContext }))
                {
                    // Fallback behavior
                    await httpContext.Response.WriteAsync("Fallback: An error occurred.");
                }
            });
        });
       
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;

                var errorResponse = new ApiResponse
                {
                    IsSuccess = false,
                    Result = null, // Fix: Set the required 'Result' property to a default value
                    StatusCode = HttpStatusCode.InternalServerError,
                    Errors = new List<ErrorResponse>
                    {
                        new ErrorResponse
                        {
                            Code = "InternalServerError", // Fix: Set required 'Code' property
                            Message = exception?.Message ?? "An unexpected error occurred.",
                            Type = "Error" // Fix: Set required 'Type' property
                        }
                    }
                };

                await context.Response.WriteAsJsonAsync(errorResponse);
            });
        });

        app.UseAuthentication();

        app.ConfigureAuthenticationEndpoints();
        app.MapCustomersEndpoints();

        app.Run();
    }
}