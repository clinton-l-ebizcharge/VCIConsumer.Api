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

public class PaymentsService : IPaymentsService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<PaymentsService> _logger;

    public PaymentsService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings, ILogger<PaymentsService> logger, IHttpContextAccessor httpContextAccessor)
    {
        var clientName = apiSettings.Value.ClientName ?? "VCIApi";
        _httpClient = httpClientFactory.CreateClient(clientName);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<PaymentListResponse>?> PaymentListAsync(PaymentListQuery paymentQuery)
    {
        _logger.LogInformation("Fetching payment list with Sort='{Sort}', Limit={Limit}, Page={Page}",
            paymentQuery.Sort, paymentQuery.LimitPerPage, paymentQuery.PageNumber);

        var queryParams = new Dictionary<string, string?>();
        queryParams.AddIfNotNullOrWhiteSpace("sort", paymentQuery.Sort);
        queryParams.AddIfHasValue("limit_per_page", paymentQuery.LimitPerPage);
        queryParams.AddIfHasValue("page_number", paymentQuery.PageNumber);

        var uriBuilder = new UriBuilder(new Uri(_httpClient.BaseAddress!, "payments"));
        uriBuilder.AddQueryParameters(queryParams);

        var response = await _httpClient.GetAsync(uriBuilder.Uri);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Payment list fetch failed. Status: {StatusCode}, Body: {Body}",
                response.StatusCode, errorBody);

            throw new HttpRequestException($"Upstream service returned {response.StatusCode}: PAYMENTS_FETCH_FAILED");
        }

        var payments = await response.Content.ReadFromJsonAsync<List<PaymentListResponse>>();

        _logger.LogInformation("Payment list fetched successfully. Count={Count}", payments?.Count ?? 0);

        return payments;
    }

    public async Task<PaymentDetailResponse?> PaymentDetailAsync(string paymentUuId)
    {
        _logger.LogInformation("Fetching payment detail. Uuid={Uuid}", paymentUuId);

        var response = await _httpClient.GetAsync($"payments/{paymentUuId}");

        if (!response.IsSuccessStatusCode)
        {
            var errorJson = await response.Content.ReadAsStringAsync();

            _logger.LogWarning("Payment detail fetch failed. Status: {StatusCode}, Body: {Body}",
                response.StatusCode, errorJson);

            try
            {
                var errorEnvelope = JsonSerializer.Deserialize<PaymentApiErrorEnvelope>(errorJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (errorEnvelope?.Errors?.Count > 0)
                {
                    var primary = errorEnvelope.Errors[0];
                    throw new InvalidOperationException($"Payment fetch error [{primary.Code}]: {primary.Message}");
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize error response. Raw={Json}", errorJson);
            }

            throw new HttpRequestException($"Upstream service returned {response.StatusCode}: PAYMENT_DETAIL_FETCH_FAILED");
        }

        var paymentDetailResponse = await response.Content.ReadFromJsonAsync<PaymentDetailResponse>();

        _logger.LogInformation("Payment detail fetch completed. Uuid={Uuid}, Found={Found}",
            paymentUuId, paymentDetailResponse is not null);

        return paymentDetailResponse;
    }


    public async Task<PaymentPostResponse?> PaymentPostAsync(PaymentPostRequest request)
    {
        //_logger.LogInformation("Creating payment. CustomerUuid={Uuid}, Amount={Amount}, SEC={Sec}",
        //    request.Customer, request.Amount, request.StandardEntryClass);

        var httpContent = JsonContent.Create(request);
        var response = await _httpClient.PostAsync("payments", httpContent);

        if (!response.IsSuccessStatusCode)
        {
            var errorJson = await response.Content.ReadAsStringAsync();

            _logger.LogWarning("Payment post failed. Status: {StatusCode}, Body: {Body}",
                response.StatusCode, errorJson);

            try
            {
                var errorEnvelope = JsonSerializer.Deserialize<PaymentApiErrorEnvelope>(errorJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (errorEnvelope?.Errors?.Count > 0)
                {
                    var primary = errorEnvelope.Errors[0];
                    throw new InvalidOperationException($"Payment post error [{primary.Code}]: {primary.Message}");
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize error response. Raw={Json}", errorJson);
            }

            throw new HttpRequestException($"Upstream service returned {response.StatusCode} errorJson: {errorJson}: PAYMENT_POST_FAILED");
        }

        var postedPayment = await response.Content.ReadFromJsonAsync<PaymentPostResponse>();

        _logger.LogInformation("Payment created successfully. UUID={Uuid}, Status={Status}",
            postedPayment?.UUId, postedPayment?.Status);

        return postedPayment;
    }

    public async Task<PaymentPostWithTokenResponse> PaymentPostWithTokenAsync(string customer_uuid, PaymentPostWithTokenRequest request)
    {
        //TODO:  CTL Add Custoemr_UUID to the request custoemr object
        //_logger.LogInformation($"Posting a Payment: {request.Name}, {request.Email}");

        var httpContent = JsonContent.Create(request);
        var response = await _httpClient.PostAsync("Payments", httpContent);
        response.EnsureSuccessStatusCode();

        var createdPayment = await response.Content.ReadFromJsonAsync<PaymentPostWithTokenResponse>();

        //_logger.LogInformation("Payment created successfully. UUID: {Uuid}", createdPayment?.UUId);
        return createdPayment!;
    }

    public async Task<PaymentUpdateResponse> PaymentUpdateAsync(string payment_uuid, PaymentUpdateRequest request)
    {
        //_logger.LogInformation("Updating Payment {Uuid}", request.UUId);

        var httpContent = JsonContent.Create(request);
        var response = await _httpClient.PatchAsync("Payments", httpContent);
        response.EnsureSuccessStatusCode();

        var updated = await response.Content.ReadFromJsonAsync<PaymentUpdateResponse>();
        return updated!;
    }    

}