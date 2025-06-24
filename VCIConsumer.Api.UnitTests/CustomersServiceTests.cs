using global::VCIConsumer.Api.Endpoints;
using global::VCIConsumer.Api.Models.Query;
using global::VCIConsumer.Api.Models.Requests;
using global::VCIConsumer.Api.Models.Responses;
using global::VCIConsumer.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using VCIConsumer.Api.Configuration;
using Xunit;

namespace VCIConsumer.Api.UnitTests.Endpoints;

public class CustomersServiceTests
{
    /// <summary>
    /// Helper method to create a test client by building a minimal host,
    /// registering a fake ICustomersService plus any required dependencies,
    /// mapping the CustomersEndpoints, and returning the test client.
    /// </summary>
    private HttpClient CreateTestClient(ICustomersService customersService)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Testing"
        });

        // Use the in–memory test server.
        builder.WebHost.UseTestServer();

        // Register required services.
        builder.Services.AddSingleton<ICustomersService>(customersService);
        builder.Services.AddHttpClient(); // Satisfy IHttpClientFactory.
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        // Add routing and endpoint explorer so model binding and routing work properly.
        builder.Services.AddRouting();
        builder.Services.AddEndpointsApiExplorer();

        // Build the app.
        var app = builder.Build();

        // Configure the middleware pipeline to include developer exception pages and routing.
        app.UseDeveloperExceptionPage();  // Shows detailed error information in tests.
        app.UseRouting();

        // Map your endpoints.
        CustomersEndpoints.MapCustomersEndpoints(app);

        // Start the app.
        app.StartAsync().GetAwaiter().GetResult();

        // Return the client from the test server.
        return app.GetTestClient();
    }

    [Fact]
    public async Task CustomerListAsync_ReturnsCustomerList_WhenResponseIsSuccessful()
    {
        //TODO:  CTL Fix
        // Arrange

        // Create a fake customer list
        var fakeCustomerList = new List<CustomerListCustomerDetailResponse>
    {
        new CustomerListCustomerDetailResponse
        {
            HRId = "001",
            UUId = "CUS_001",
            Name = "Test Customer 1",
            EmailTruncated = "t**@email.com",
            PhoneTruncated = "XXX-XXX-0001"
        },
        new CustomerListCustomerDetailResponse
        {
            HRId = "002",
            UUId = "CUS_002",
            Name = "Test Customer 2",
            EmailTruncated = "a**@email.com",
            PhoneTruncated = "XXX-XXX-0002"
        }
    };

        // Serialize the fake customer list into JSON.
        var json = JsonSerializer.Serialize(fakeCustomerList);
        var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        // Set up a mocked HttpMessageHandler to intercept outgoing HTTP requests.
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(fakeResponse)
            .Verifiable();

        // Create an HttpClient with a base address.
        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://fakeapi.com/")
        };

        // Set up a mocked IHttpClientFactory that returns our HttpClient.
        var clientFactoryMock = new Mock<IHttpClientFactory>();
        clientFactoryMock.Setup(cf => cf.CreateClient(It.Is<string>(s => s == "VCIApi")))
                         .Returns(httpClient);

        // Set up IOptions<ApiSettings> with the required ClientName.
        var apiSettings = Options.Create(new ApiSettings
        {
            ClientName = "VCIApi"
            // Additional API settings can be added here if needed.
        });

        // Use a null logger (or use your own mocked logger as needed).
        ILogger<CustomersService> logger = NullLogger<CustomersService>.Instance;

        // Create a default HttpContextAccessor (or a mocked version if necessary).
        var httpContextAccessor = new Microsoft.AspNetCore.Http.HttpContextAccessor();

        // Instantiate the CustomersService with the mocked dependencies.
        var service = new CustomersService(clientFactoryMock.Object, apiSettings, logger, httpContextAccessor);

        // Update the property name to match the correct definition in CustomerListQuery.
        var query = new CustomerListQuery
        {
            Sort = "name",
            LimitPerPage = 10, 
            PageNumber = 1     
        };

        // Act
        var result = await service.CustomerListAsync(query);

        // Assert
        //TODO:  CTL Fix
        Assert.NotNull(result);
        //Assert.Equal(fakeCustomerList.Count, result.Count);
        //Assert.Equal(fakeCustomerList[0].UUId, result[0].UUId);
        //Assert.Equal(fakeCustomerList[0].Name, result[0].Name);

        // Optionally, verify that the sample HTTP GET was called correctly.
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri.ToString().StartsWith("http://fakeapi.com/customers")),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task CustomerDetailEndpoint_ReturnsSuccessResponse_WithCustomerDetail()
    {
        // Arrange: Create a fake customer detail.
        var sampleUuid = "CUS_1253343761799155712900095XNTC";
        var fakeDetail = new CustomerDetailResponse
        {
            UUId = sampleUuid,
            Name = "Jane Doe"
        };

        var mockService = new Mock<ICustomersService>();
        mockService.Setup(svc => svc.CustomerDetailAsync(It.IsAny<string>()))
                   .ReturnsAsync(fakeDetail);

        var client = CreateTestClient(mockService.Object);

        // Act: Call GET /customers/{customer_uuid}.
        var response = await client.GetAsync($"/customers/{sampleUuid}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        // Assert: Verify that the returned JSON contains the expected customer details.
        using var doc = JsonDocument.Parse(json);
        var success = doc.RootElement.GetProperty("isSuccess").GetBoolean();
        Assert.True(success);



        var data = doc.RootElement.GetProperty("result");
        Assert.Equal(fakeDetail.UUId, data.GetProperty("uuid").GetString());
        Assert.Equal(fakeDetail.Name, data.GetProperty("name").GetString());
    }

    [Fact]
    public async Task CustomerCreationEndpoint_ReturnsSuccessResponse_WithCreatedCustomer()
    {
        // Arrange: Set up a fake customer creation response.
        var fakeCreationResponse = new CustomerCreationResponse
        {
            UUId = "CUS_NEW"
            // Populate additional properties as needed.
        };

        var mockService = new Mock<ICustomersService>();
        mockService.Setup(svc => svc.CustomerCreationAsync(It.IsAny<CustomerCreationRequest>()))
                   .ReturnsAsync(fakeCreationResponse);

        var client = CreateTestClient(mockService.Object);

        // Prepare a sample CustomerCreationRequest.
        var newCustomerRequest = new CustomerCreationRequest
        {
            Name = "New Customer",
            Email = "new@customer.com"
            // Include any other required properties.
        };

        var content = JsonContent.Create(newCustomerRequest);

        // Act: Post to /customers.
        var response = await client.PostAsync("/customers", content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        // Assert: The response should be a success response with the created customer data.
        using var doc = JsonDocument.Parse(json);
        var success = doc.RootElement.GetProperty("isSuccess").GetBoolean();
        Assert.True(success);

        var resultElement = doc.RootElement.GetProperty("result");
        var uuid = resultElement.GetProperty("uuid").GetString();
        Assert.Equal(fakeCreationResponse.UUId, uuid);
    }

    [Fact]
    public async Task CustomerUpdateEndpoint_ReturnsSuccessResponse_WithUpdatedCustomer()
    {
        // Arrange: Create a fake update response.
        var fakeUpdateResponse = new CustomerUpdateResponse
        {
            UUId = "CUS_UPDATED"
            // Populate additional properties as needed.
        };

        var mockService = new Mock<ICustomersService>();
        mockService.Setup(svc => svc.CustomerUpdateAsync(It.IsAny<CustomerUpdateRequest>()))
                   .ReturnsAsync(fakeUpdateResponse);

        var client = CreateTestClient(mockService.Object);

        // Prepare a sample CustomerUpdateRequest.
        var updateRequest = new CustomerUpdateRequest
        {
            Name = "Updated Customer",
            UUId = "CUS_UPDATED"
            // Set other properties as needed.
        };

        var content = JsonContent.Create(updateRequest);

        // Act: Because the endpoint is mapped as PATCH, create an HttpRequestMessage using the PATCH method.
        var requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), "/customers")
        {
            Content = content
        };

        var response = await client.SendAsync(requestMessage);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        // Assert: Verify that the update response is wrapped correctly.
        using var doc = JsonDocument.Parse(json);
        var success = doc.RootElement.GetProperty("isSuccess").GetBoolean();
        Assert.True(success);

        var resultElement = doc.RootElement.GetProperty("result");
        var uuid = resultElement.GetProperty("uuid").GetString();
        Assert.Equal(fakeUpdateResponse.UUId, uuid);
    }
}
