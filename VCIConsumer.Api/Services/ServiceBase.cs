using Newtonsoft.Json;
using System.Net.Http.Headers;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Services;

public class ServiceBase
{
    public static string SecretKey { get; set; } = string.Empty;
    public static AccessToken AccessToken { get; set; } = null!; 
    public static string ClientId { get; set; } = string.Empty;
    public static bool Live { get; set; } = false;
    public string Endpoint { get; set; } = string.Empty;

    public ServiceBase(string clientId, string secretKey)
    {
    }

    public ServiceBase()
    {
    }

    protected async Task<ApiResponse> GetAccessToken()
    {
        var http = CreateHTTPClient();
        var content = CreateHttpContent(new { client_id = ClientId, client_secret = SecretKey });
        var httpResponseMessage = await http.PostAsync("authentication", content);
        var response = ProcessResponse<AccessToken>(httpResponseMessage);
        
        ApiResponse apiResponse = new()
        {
            StatusCode = httpResponseMessage.StatusCode,
            IsSuccess = httpResponseMessage.IsSuccessStatusCode,
            Result = response, // Fix: Set the required 'Result' property
            Errors = new List<ErrorResponse>() // Initialize Errors to avoid null reference issues
        };
        
        return apiResponse;
    }
    protected HttpContent CreateHttpContent(object data)
    {
        var json = JsonConvert.SerializeObject(data);
        HttpContent httpContent = new StringContent(json);
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return httpContent;
    }

    protected HttpClient CreateHTTPClient()
    {
        HttpClient ret = new HttpClient() { BaseAddress = new Uri(Endpoint) };
        ret.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (AccessToken != null)
            ret.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AccessToken.TokenType.Trim(), AccessToken.Token);

        var guid = Guid.NewGuid().ToString();
        ret.DefaultRequestHeaders.Add("Idempotency-Key", $"{guid}");
        
        return ret;
    }
    protected ApiResponse ProcessResponse<TData>(HttpResponseMessage httpResponseMessage)
    {
        ApiResponse apiResponse = new()
        {
            StatusCode = httpResponseMessage.StatusCode,
            IsSuccess = httpResponseMessage.IsSuccessStatusCode,
            Result = httpResponseMessage, // Fix: Set the required 'Result' property
            Errors = new List<ErrorResponse>() // Initialize Errors to avoid null reference issues
        };

        var response = new APIResponseResult<TData>(httpResponseMessage);

        if (response.OK)
        {
            apiResponse.IsSuccess = true;
            apiResponse.StatusCode = System.Net.HttpStatusCode.OK; // Set to OK if the response is successful
            apiResponse.Result = response.DataObject!;
        }
        else
            apiResponse.Errors = ProcessError(httpResponseMessage)
                .Select(error => new ErrorResponse // Convert ErrorMessage[] to List<ErrorResponse>
                {
                    Code = error.code,
                    Message = error.message,
                    Type = error.type
                })
                .ToList();

        return apiResponse;
    }
    protected ErrorMessage[] ProcessError(HttpResponseMessage mm)
    {
        var rs = new APIResponseResult<VCIError>(mm);
        return rs.DataObject.Errors;
    }

    protected async Task<ApiResponse> APIPatch<TData>(string url, HttpContent data, int trialCount = 2)
    {
        return await APITry<TData>("PATCH", url, data, trialCount = 2);
    }

    protected async Task<ApiResponse> APIGet<TData>(string url, int trialCount = 2)
    {
        return await APITry<TData>("GET", url, null, trialCount = 2);
    }
    protected async Task<ApiResponse> APIPost<TData>(string url, HttpContent httpContent, int trialCount = 2)
    {
        var json = await httpContent.ReadAsStringAsync();
        var apiResponse = await APITry<TData>("POST", url, httpContent, trialCount = 2);
        
        return apiResponse;
    }
    protected async Task<ApiResponse> APITry<TData>(string Method, string url, HttpContent? data, int trialCount = 2)
    {
        if (AccessToken == null || !AccessToken.IsValid)
            await GetAccessToken();

        int counter = 0;
        ApiResponse apiResponse = null!;
        var http = CreateHTTPClient();
        while (++counter < trialCount)
        {
            try
            {
                HttpResponseMessage httpResponseMessage = null!;
                switch (Method)
                {
                    case "GET":
                        httpResponseMessage = await http.GetAsync(url);
                        break;
                    case "POST":
                        httpResponseMessage = await http.PostAsync(url, data);
                        break;
                    default:
                        HttpRequestMessage rq = new HttpRequestMessage(new HttpMethod(Method), url)
                        {
                            Content = data
                        };
                        httpResponseMessage = await http.SendAsync(rq);
                        break;
                }

                var response = ProcessResponse<TData>(httpResponseMessage);

                // In case of access token expired
                //TODO:  CTL
                //if (response != null && response.HasError && response.Errors.Any(x => x.type == "TOKEN_EXPIRED"))
                //    await GetAccessToken(); // Get access token and repeat
                //else
                //    break;
            }
            catch (Exception ex)
            {
                if (counter >= trialCount)
                {
                    //ret = new ResponseResult<TData>
                    //{
                    //    Data = default!, // Set default value for Data
                    //    JSON = string.Empty, // Set an empty string for JSON
                    //    Errors = new ErrorMessage[] { new ErrorMessage { code = "00", message = ex.Message, type = "Lib internal" } }
                    //};
                }

                Thread.Sleep(250);
            }
        }

        return apiResponse;
    }
}
