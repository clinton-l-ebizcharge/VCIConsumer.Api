using Microsoft.Extensions.Options;
using Newtonsoft.Json; // If you're using Newtonsoft.Json for serialization
using System.Net.Http.Headers;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Services;

public abstract class ServiceBase
{
    private readonly ApiSettings _apiSettings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TokenService _tokenService; // Inject TokenService

    protected ServiceBase(IOptions<ApiSettings> apiSettings, IHttpClientFactory httpClientFactory, TokenService tokenService)
    {
        _apiSettings = apiSettings.Value;
        _httpClientFactory = httpClientFactory;
        _tokenService = tokenService;
    }

    protected HttpContent CreateHttpContent(object data)
    {
        var json = JsonConvert.SerializeObject(data);
        HttpContent httpContent = new StringContent(json);
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return httpContent;
    }

    protected async Task<ApiResponse> APIGet<T>(string endpoint)
    {
        var client = _httpClientFactory.CreateClient("VCIApi");
        var accessToken = await _tokenService.GetTokenAsync(); // Get token  
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); // Add token to header  

        var response = await client.GetAsync($"{_apiSettings.BaseUrl}/{endpoint}");
        return await ProcessApiResponse<T>(response); // Specify the type argument explicitly  
    }

    protected async Task<ApiResponse> APIPost<T>(string endpoint, HttpContent content)
    {
        var client = _httpClientFactory.CreateClient("VCIApi");
        var accessToken = await _tokenService.GetTokenAsync(); // Get token  
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); // Add token to header  

        var response = await client.PostAsync($"{_apiSettings.BaseUrl}/{endpoint}", content);
        return await ProcessApiResponse<T>(response); // Specify the type argument explicitly  
    }

    protected async Task<ApiResponse> APIPatch<T>(string endpoint, HttpContent content)
    {
        var client = _httpClientFactory.CreateClient("VCIApi");
        var accessToken = await _tokenService.GetTokenAsync(); // Get token  
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); // Add token to header  

        var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{_apiSettings.BaseUrl}/{endpoint}")
        {
            Content = content
        };
        var response = await client.SendAsync(request);
        return await ProcessApiResponse<T>(response); // Specify the type argument explicitly  
    }

    private async Task<ApiResponse> ProcessApiResponse<T>(HttpResponseMessage response)
    {
        var apiResponse = new ApiResponse
        {
            Result = default(T), // Initialize the required member 'Result'
            Errors = new List<ErrorResponse>() // Ensure 'Errors' is initialized
        };

        if (response.IsSuccessStatusCode)
        {
            apiResponse.Result = await response.Content.ReadFromJsonAsync<T>();
            apiResponse.IsSuccess = true;
        }
        else
        {
            apiResponse.IsSuccess = false;
            var errorContent = await response.Content.ReadAsStringAsync();
            apiResponse.Errors.Add(new ErrorResponse
            {
                Code = "HTTP_ERROR", // Provide a default value for 'Code'
                Type = "ResponseError", // Provide a default value for 'Type'
                Message = $"Error: {response.StatusCode} - {errorContent}"
            });
        }

        return apiResponse;
    }
}