using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Services;

public class PaymentsService 
{
    private readonly ApiSettings _apiSettings;
    private readonly IHttpClientFactory _httpClientFactory;

    public PaymentsService(IOptions<ApiSettings> apiSettings, IHttpClientFactory httpClientFactory, TokenService tokenService)
       
    {
        _apiSettings = apiSettings.Value;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ApiResponse> PaymentList(Payment.PaymentStatuses paymentStatus, DateTime fromDate, DateTime todate)
    {
        Dictionary<string, string> DateParas = new Dictionary<string, string>();
        string fromDatestr = $"{fromDate.Year}-{fromDate.Month.ToString("D2")}-{fromDate.Day.ToString("D2")} {fromDate.Hour.ToString("D2")}:{fromDate.Minute.ToString("D2")}:{fromDate.Second.ToString("D2")}";
        string toDatestr = $"{todate.Year}-{todate.Month.ToString("D2")}-{todate.Day.ToString("D2")} {todate.Hour.ToString("D2")}:{todate.Minute.ToString("D2")}:{todate.Second.ToString("D2")}";
        switch (paymentStatus)
        {
            case Payment.PaymentStatuses.ORIGINATED:
                DateParas.Add("originate_at[gte]", fromDatestr);
                DateParas.Add("originate_at[lte]", toDatestr);
                break;
            case Payment.PaymentStatuses.SETTLED:
            case Payment.PaymentStatuses.PARTIAL_SETTLED:
                DateParas.Add("settled_at[gte]", fromDatestr);
                DateParas.Add("settled_at[lte]", toDatestr);
                break;
            case Payment.PaymentStatuses.RETURN:
                DateParas.Add("returned_at[gte]", fromDatestr);
                DateParas.Add("returned_at[lte]", toDatestr);
                break;
            default:
                DateParas.Add("created_at[gte]", fromDatestr);
                DateParas.Add("created_at[lte]", toDatestr);
                break;
        }
        StringBuilder strb = new StringBuilder();
        strb.Append("payments?");
        strb.Append($"status={paymentStatus.ToString().Replace("_", " ")}");
        foreach (var kv in DateParas)
        {
            strb.Append("&");
            strb.Append(kv.Key);
            strb.Append("=");
            strb.Append(kv.Value);
        }
        
        var apiResponse = await APIGet<Payment[]>(strb.ToString());
        return apiResponse;
    }

    public async Task<ApiResponse> PaymentDetail(string paymentId)
    {
        int i = paymentId.IndexOf("_");
        string urlPath = "payments";
        switch (paymentId.Substring(0, i))
        {
            case "RFN":
                urlPath = "refunds";
                break;
            case "POT":
                urlPath = "payouts";
                break;
            case "PMT":
                urlPath = "payments";
                break;
        }

        var rs = await APIGet<Payment>($"{urlPath}/{paymentId}");
        return rs;
    }

    public async Task<ApiResponse> PaymentCreate(_Customer c, PaymentBase.PaymentEntries entryClass, double amount, _Check check, string description)
    {
        var requestData = new
        {
            city = "XXXX",
            state = "YY",
            customer = c,
            check,
            amount = Math.Round(amount, 2),
            standard_entry_class = entryClass.ToString().Replace("_", " "),
            description
        };

        var httpContent = CreateHttpContent(requestData);
        var ret = await APIPost<_PaymentResult>("payments", httpContent);

        return ret;
    }

    public async Task<ApiResponse> PaymentVoid(string paymentId)
    {
        var rs = await APIPatch<_PaymentResult>($"payments/{paymentId}", CreateHttpContent(new { status = Payment.PaymentStatuses.VOID.ToString().Replace("_", " ") }));
        return rs;
    }

    public async Task<ApiResponse> PaymentUpdate(string paymentId, Payment.PaymentStatuses status)
    {
        var rs = await APIPatch<_PaymentResult>($"payments/{paymentId}", CreateHttpContent(new { status = status.ToString().Replace("_", " ") }));
        return rs;
    }
}
