using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Extensions;
using VCIConsumer.Api.Models.Query;
using VCIConsumer.Api.Models.Requests;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Services;

public class CustomersService : ICustomersService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CustomersService> _logger;

    public CustomersService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings, ILogger<CustomersService> logger, IHttpContextAccessor httpContextAccessor)
    {
        var clientName = apiSettings.Value.ClientName ?? "VCIApi";
        _httpClient = httpClientFactory.CreateClient(clientName);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CustomerListResponse?> CustomerListAsync(CustomerListQuery customerQuery)
    {
        _logger.LogSummary(customerQuery); 

        var queryParams = new Dictionary<string, string?>();

        queryParams.AddIfNotNullOrWhiteSpace("sort", customerQuery.Sort);
        queryParams.AddIfHasValue("limit_per_page", customerQuery.LimitPerPage);
        queryParams.AddIfHasValue("page_number", customerQuery.PageNumber);

        var uriBuilder = new UriBuilder(new Uri(_httpClient.BaseAddress!, "customers"));
        uriBuilder.AddQueryParameters(queryParams);

        var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
        request.Headers.Add("Vericheck-Version", "1");

        var response = await _httpClient.GetAsync(uriBuilder.Uri);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Customer list fetch failed. Status: {StatusCode}, Body: {Body}",
                response.StatusCode, errorBody);

            throw new HttpRequestException($"Upstream service returned {response.StatusCode}: CUSTOMERS_FETCH_FAILED");
        }

        var customerListResponse = await response.Content.ReadFromJsonAsync<CustomerListResponse>();

        if (customerListResponse is null)
        {
            _logger.LogError("Customer list fetch returned null.");
            throw new InvalidOperationException("Customer list fetch failed: null response.");
        }

        _logger.LogInformation("Customer list fetched successfully. Count={Count}", customerListResponse.Customers.Count);
        return customerListResponse;
    }


    public async Task<CustomerDetailResponse> CustomerDetailAsync(string customerUuid)
    {
        _logger.LogInformation("Fetching customer detail for {CustomerUuid}", customerUuid);

        var uri = new Uri(_httpClient.BaseAddress + $"customers/{customerUuid}");

        var response = await _httpClient.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Customer detail fetch failed. Uuid={Uuid} | Status={StatusCode}, Body={Body}",
                customerUuid, response.StatusCode, errorBody);

            throw new HttpRequestException($"CUSTOMER_DETAIL_FETCH_FAILED ({(int)response.StatusCode})");
        }

        var customer = await response.Content.ReadFromJsonAsync<CustomerDetailResponse>();

        if (customer == null)
        {
            _logger.LogError("Customer detail response deserialized as null. uuid={uuid}", customerUuid);
            throw new InvalidOperationException("Customer detail fetch failed: null response.");
        }

        _logger.LogInformation("Customer detail fetched successfully. Name={Name}, uuid={Uuid}",
            customer.Name, customer.UUId);

        return customer;
    }

    public async Task<CustomerCreationResponse> CustomerCreationAsync(CustomerCreationRequest request)
    {
        _logger.LogInformation("Creating new customer: {Name}, {Email}", request.Name, request.Email);

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "customers")
        {
            Content = JsonContent.Create(request)
        };

        httpRequest.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

        var response = await _httpClient.SendAsync(httpRequest);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Customer creation failed. Status: {StatusCode}, Body: {Body}", response.StatusCode, errorBody);
            throw new HttpRequestException($"Upstream service returned {response.StatusCode}: CUSTOMER_CREATE_FAILED");
        }

        var result = await response.Content.ReadFromJsonAsync<CustomerCreationResponse>();

        if (result is null)
        {
            _logger.LogError("Customer creation response was null.");
            throw new InvalidOperationException("Customer creation failed: null response.");
        }

        _logger.LogInformation("Customer created successfully. uuid={uuid}", result.UUId);
        return result;
    }


    public async Task<CustomerUpdateResponse> CustomerUpdateAsync(CustomerUpdateRequest request)
    {
        _logger.LogInformation("Updating customer {uuid}", request.UUId);

        var httpRequest = new HttpRequestMessage(HttpMethod.Patch, "customers")
        {
            Content = JsonContent.Create(request)
        };

        httpRequest.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

        var response = await _httpClient.SendAsync(httpRequest);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Customer update failed. Status: {StatusCode}, Body: {Body}",
                response.StatusCode, errorBody);

            throw new HttpRequestException($"Upstream service returned {response.StatusCode}: CUSTOMER_UPDATE_FAILED");
        }

        var updated = await response.Content.ReadFromJsonAsync<CustomerUpdateResponse>();

        if (updated is null)
        {
            _logger.LogError("Customer update returned null.");
            throw new InvalidOperationException("Customer update failed: null response.");
        }

        _logger.LogInformation("Customer updated successfully. uuid={uuid}", updated.UUId);
        return updated;
    }

}