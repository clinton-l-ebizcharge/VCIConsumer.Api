using Microsoft.AspNetCore.Mvc;
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

    public async Task<List<CustomerListResponse>> CustomerListAsync(CustomerListQuery customerQuery)
    {
        _logger.LogInformation("Fetching customer list with Sort='{Sort}', Limit={Limit}, Page={PageNumber}",
            customerQuery.Sort, customerQuery.Limit_Per_Page, customerQuery.Page_Number);

        var queryParams = new Dictionary<string, string?>();

        if (!string.IsNullOrWhiteSpace(customerQuery.Sort))
            queryParams["sort"] = customerQuery.Sort;

        if (!string.IsNullOrWhiteSpace(customerQuery.Limit_Per_Page))
            queryParams["limit_per_page"] = customerQuery.Limit_Per_Page;

        if (!string.IsNullOrWhiteSpace(customerQuery.Page_Number))
            queryParams["page_number"] = customerQuery.Page_Number;

        var uriBuilder = new UriBuilder(_httpClient.BaseAddress + "customers");
        uriBuilder.AddQueryParameters(queryParams);

        var response = await _httpClient.GetAsync(uriBuilder.Uri);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Customer list fetch failed. Status: {StatusCode}, Body: {Body}",
                response.StatusCode, errorBody);

            throw new HttpRequestException($"Upstream service returned {response.StatusCode}: CUSTOMERS_FETCH_FAILED");
        }

        var customers = await response.Content.ReadFromJsonAsync<List<CustomerListResponse>>();

        if (customers == null)
        {
            _logger.LogError("Customer list fetch returned null.");
            throw new InvalidOperationException("Customer list fetch failed: null response.");
        }

        _logger.LogInformation("Customer list fetched successfully. Count={Count}", customers.Count);

        return customers;
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
            _logger.LogError("Customer detail response deserialized as null. Uuid={Uuid}", customerUuid);
            throw new InvalidOperationException("Customer detail fetch failed: null response.");
        }

        _logger.LogInformation("Customer detail fetched successfully. Name={Name}, Uuid={Uuid}",
            customer.Name, customer.UUId);

        return customer;
    }

    public async Task<CustomerCreationResponse> CustomerCreationAsync(CustomerCreationRequest request)
    {
        _logger.LogInformation($"Creating new customer: {request.Name}, {request.Email}");

        var httpContent = JsonContent.Create(request);
        var response = await _httpClient.PostAsync("customers", httpContent);
        response.EnsureSuccessStatusCode();

        var createdCustomer = await response.Content.ReadFromJsonAsync<CustomerCreationResponse>();

        _logger.LogInformation("Customer created successfully. UUID: {Uuid}", createdCustomer?.UUId);
        return createdCustomer!;
    }

    public async Task<CustomerUpdateResponse> CustomerUpdateAsync(CustomerUpdateRequest request)
    {
        _logger.LogInformation("Updating customer {Uuid}", request.Uuid);

        var httpContent = JsonContent.Create(request);
        var response = await _httpClient.PatchAsync("customers", httpContent);
        response.EnsureSuccessStatusCode();

        var updated = await response.Content.ReadFromJsonAsync<CustomerUpdateResponse>();
        return updated!;
    }
}