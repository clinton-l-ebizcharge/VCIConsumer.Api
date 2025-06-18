using Microsoft.AspNetCore.Http.Json;
using Scalar.AspNetCore;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Endpoints;
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

        builder.Services.AddScoped<ICustomersService, CustomersService>();
        builder.Services.AddSingleton<TokenService>();

        builder.Services.AddAntiforgery();

        builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
        var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var apiClientName = configurationBuilder.Build().GetValue<string>("ApiSettings:ApiClientName") ?? "VCIApi";

        builder.Services.AddHttpClient(apiClientName, client =>
        {
            var baseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://api.sandbox.vericheck.com";
            client.BaseAddress = new Uri(baseUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("Idempotency-Key", Guid.NewGuid().ToString()); // Convert Guid to string


            client.DefaultRequestHeaders.Add("VeriCheck-Version", "1.13");
            client.DefaultRequestHeaders.Add("Content-Type", "application/json");
        });

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
                options.WithForceThemeMode(ThemeMode.Dark); // Default to dark mode if preferred
                options.WithOpenApiRoutePattern("/openapi/v1.json");

                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.AsyncHttp);
            });
        }
        ;

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

        app.UseAuthentication();

        app.ConfigureAuthenticationEndpoints();
        app.ConfigureCustomersEndpoints();

        app.Run();
    }
}