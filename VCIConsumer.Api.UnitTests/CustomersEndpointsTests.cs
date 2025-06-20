using global::VCIConsumer.Api.Endpoints;
using global::VCIConsumer.Api.Models.Query;
using global::VCIConsumer.Api.Models.Requests;
using global::VCIConsumer.Api.Models.Responses;
using global::VCIConsumer.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace VCIConsumer.Api.UnitTests.Endpoints;

public class CustomersEndpointsTests
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

        // Use the in-memory test server.
        builder.WebHost.UseTestServer(); // Ensure the TestServer package is referenced in your project.

        // Register the minimal dependencies:
        builder.Services.AddSingleton<ICustomersService>(customersService);
        builder.Services.AddHttpClient(); // To satisfy IHttpClientFactory.
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        // Build the app and map the endpoints.
        var app = builder.Build();
        CustomersEndpoints.MapCustomersEndpoints(app);

        // Start the host.
        app.StartAsync().GetAwaiter().GetResult();

        // Return the TestServer HttpClient.
        return app.GetTestClient();
    }

    [Fact]
    public async Task CustomerListEndpoint_ReturnsSuccessResponse_WithCustomerList()
    {
        // Arrange: Create a fake customer list.  
        string json = @"
                   [
                    {
                      ""hrid"": ""RKQ2PL28V936BDV"",
                      ""uuid"": ""CUS_1253058024894410752900095XNTC"",
                      ""name"": ""customer003"",
                      ""email_truncated"": ""c**@123.com"",
                      ""phone_truncated"": ""XXX-XXX-0003"",
                      ""bank_account"": {
                        ""account_type"": ""Savings"",
                        ""account_validated"": false,
                        ""account_number_truncated"": ""0003"",
                        ""routing_number_truncated"": ""0006""
                      },
                      ""created_at"": ""2025-06-19 14:45:13 ET"",
                      ""modified_at"": ""2025-06-19 14:45:13 ET"",
                      ""active"": true
                    },
                    {
                      ""hrid"": ""FLG4AHUTNYJQRFL"",
                      ""uuid"": ""CUS_1253055232913764352900095XNTC"",
                      ""name"": ""Customer002"",
                      ""email_truncated"": ""c**@123.com"",
                      ""phone_truncated"": ""XXX-XXX-0002"",
                      ""bank_account"": {
                        ""account_type"": ""CHECKING"",
                        ""account_validated"": false,
                        ""account_number_truncated"": ""0002"",
                        ""routing_number_truncated"": ""0006""
                      },
                      ""created_at"": ""2025-06-19 14:34:08 ET"",
                      ""modified_at"": ""2025-06-19 14:34:08 ET"",
                      ""active"": true
                    },
                    {
                      ""hrid"": ""W8ZQQX4FYVMHVLS"",
                      ""uuid"": ""CUS_1253054927467499520900095XNTC"",
                      ""name"": ""Customer001"",
                      ""email_truncated"": ""c**@123.com"",
                      ""phone_truncated"": ""XXX-XXX-0001"",
                      ""bank_account"": {
                        ""account_type"": ""CHECKING"",
                        ""account_validated"": false,
                        ""account_number_truncated"": ""0001"",
                        ""routing_number_truncated"": ""0006""
                      },
                      ""created_at"": ""2025-06-19 14:32:55 ET"",
                      ""modified_at"": ""2025-06-19 14:35:11 ET"",
                      ""active"": true
                    }
                   ]";


        var fakeCustomerList = JsonSerializer.Deserialize<List<CustomerListResponse>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Ensure fakeCustomerList is not null.
        Assert.NotNull(fakeCustomerList);

        var mockService = new Mock<ICustomersService>();
        mockService.Setup(svc => svc.CustomerListAsync(It.IsAny<CustomerListQuery>()))
                   .ReturnsAsync(fakeCustomerList!);

        var client = CreateTestClient(mockService.Object);

        // Act: Invoke GET /customers (the list endpoint).  
        var response = await client.GetAsync("/customers");
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();

        // Assert: We expect a JSON object with Success=true and a Data array containing our list.  
        using var doc = JsonDocument.Parse(jsonResponse);
        var success = doc.RootElement.GetProperty("isSuccess").GetBoolean();
        Assert.True(success, "Expected the success flag to be true");

        var data = doc.RootElement.GetProperty("result");
        Assert.Equal(fakeCustomerList!.Count, data.GetArrayLength());
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
