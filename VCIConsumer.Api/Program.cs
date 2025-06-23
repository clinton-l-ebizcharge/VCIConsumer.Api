using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http.Json;
using Scalar.AspNetCore;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Endpoints;
using VCIConsumer.Api.Extensions;
using VCIConsumer.Api.Filter;
using VCIConsumer.Api.Handler;
using VCIConsumer.Api.Models;
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
        builder.Services.AddSwaggerGen(s =>
        {
            s.UseInlineDefinitionsForEnums();
        });

        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<PaymentPostRequestValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<CustomerListQueryValidator>();
        builder.Services.AddScoped(typeof(StandardValidatedApiFilter<>));

        builder.Services.AddScoped<ICustomersService, CustomersService>();
        builder.Services.AddScoped<IPaymentsService, PaymentsService>();
        builder.Services.AddSingleton<ITokenService, TokenService>();
        builder.Services.AddSingleton<ITimeProvider, SystemTimeProvider>();
        
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

        app.UseStandardApiExceptionHandler();

        app.UseAuthentication();
        
        app.MapCustomersEndpoints();
        app.MapPaymentsEndpoints();

        app.Run();
    }
}