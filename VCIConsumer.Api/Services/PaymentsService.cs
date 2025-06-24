using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Extensions;
using VCIConsumer.Api.Handler;
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

    public async Task<PaymentListResponse?> PaymentListAsync(PaymentListQuery paymentQuery)
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

        //If unsure of response - Use to DEBUG
        var raw = await response.Content.ReadAsStringAsync();
        _logger.LogWarning("Raw customer JSON: {Json}", raw);

        var paymentListResponse = await response.Content.ReadFromJsonAsync<PaymentListResponse>();

        _logger.LogInformation("Payment list fetched successfully. Count={Count}", paymentListResponse?.Payments?.Count ?? 0);

        return paymentListResponse;
    }

    public async Task<PaymentDetailResponse?> PaymentDetailAsync(string payment_uuid)
    {
        _logger.LogInformation("Fetching payment detail. Uuid={Uuid}", payment_uuid);

        var response = await _httpClient.GetAsync($"payments/{payment_uuid}");

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
            payment_uuid, paymentDetailResponse is not null);

        return paymentDetailResponse;
    }


    public async Task<PaymentPostResponse?> PaymentPostAsync(PaymentPostRequest request)
    {
        //_logger.LogInformation("Creating payment. CustomerUuid={Uuid}, Amount={Amount}, SEC={Sec}",
        //    request.Customer, request.Amount, request.StandardEntryClass);

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "payments")
        {
            Content = JsonContent.Create(request)
        };

        httpRequest.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

        var httpContent = JsonContent.Create(request);
        var response = await _httpClient.SendAsync(httpRequest);
        
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

    public async Task<PaymentPostWithTokenResponse?> PaymentPostWithTokenAsync(string customer_uuid, PaymentPostWithTokenRequest request)
    {

        _logger.LogInformation("Posting payment with token. CustomerUuid={Uuid}, Amount={Amount}, SEC={Sec}",
            customer_uuid, request.Amount, request.StandardEntryClass);

        request.Customer ??= new PaymentPostWithTokenCustomerRequest() { UUId = customer_uuid };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "payments")
        {
            Content = JsonContent.Create(request)
        };

        httpRequest.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
        var response = await _httpClient.SendAsync(httpRequest);

        if ((int)response.StatusCode >= 500 && (int)response.StatusCode < 600)
        {
            return (PaymentPostWithTokenResponse?)(object?)await UpstreamResponseHandler.HandleFailureAsync(
                response, _logger, "PaymentPostWithToken");
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Payment post failed. Status={StatusCode}, Body={Body}",
                response.StatusCode, errorBody);

            throw new HttpRequestException($"Payment post failed with status {(int)response.StatusCode}: {response.ReasonPhrase}");
        }

        //If unsure of response - Use to DEBUG
        var raw = await response.Content.ReadAsStringAsync();
        _logger.LogWarning("Raw customer JSON: {Json}", raw);

        var createdPayment = await response.Content.ReadFromJsonAsync<PaymentPostWithTokenResponse>();

        //_logger.LogInformation("Payment with token created successfully. UUID={Uuid}, Status={Status}",
        //    createdPayment?., createdPayment?.Status);

        return createdPayment;
    }


    public async Task<PaymentUpdateResponse?> PaymentUpdateAsync(string paymentUuid, PaymentUpdateRequest request)
    {
        _logger.LogInformation("Updating payment. Uuid={Uuid}", paymentUuid);

        var httpRequest = new HttpRequestMessage(HttpMethod.Patch, "payments")
        {
            Content = JsonContent.Create(request)
        };

        httpRequest.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

        var response = await _httpClient.SendAsync(httpRequest);

        if ((int)response.StatusCode >= 500 && (int)response.StatusCode < 600)
        {
            await UpstreamResponseHandler.HandleFailureAsync(response, _logger, "PaymentUpdate");
            throw new HttpRequestException($"Upstream service returned {response.StatusCode}: PAYMENT_UPDATE_FAILED");
        }

        if (!response.IsSuccessStatusCode)
        {
            var rawError = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Payment update failed. Status={StatusCode}, Body={Body}", response.StatusCode, rawError);
            throw new HttpRequestException($"Payment update failed with status {(int)response.StatusCode}");
        }

        var updated = await response.Content.ReadFromJsonAsync<PaymentUpdateResponse>();

        //TODO:  CTL Fix
        //_logger.LogInformation("Payment update completed. Uuid={Uuid}, Status={Status}",

        return updated;
    }


}