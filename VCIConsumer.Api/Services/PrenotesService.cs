using Microsoft.Extensions.Options;
using System.Text;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Services;

public class PrenotesService : ServiceBase
{
    private readonly ApiSettings _apiSettings;
    private readonly IHttpClientFactory _httpClientFactory;

    public PrenotesService(IOptions<ApiSettings> apiSettings, IHttpClientFactory httpClientFactory, TokenService tokenService)
        : base(apiSettings, httpClientFactory, tokenService)
    {
        _apiSettings = apiSettings.Value;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ApiResponse> PrenoteList(Payment.PaymentStatuses paymentStatus, DateTime fromDate, DateTime todate)
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
        strb.Append("prenotes?");
        strb.Append($"status={paymentStatus.ToString().Replace("_", " ")}");
        foreach (var kv in DateParas)
        {
            strb.Append("&");
            strb.Append(kv.Key);
            strb.Append("=");
            strb.Append(kv.Value);
        }

        var rs = await APIGet<Payment[]>(strb.ToString());
        return rs;
    }

    public async Task<ApiResponse> PrenoteDetail(string prenoteId)
    {
        prenoteId = prenoteId.StartsWith("NTE_", StringComparison.OrdinalIgnoreCase) ? prenoteId : $"NTE_{prenoteId}";
        var rs = await APIGet<Payment>($"prenotes/{prenoteId}");
        return rs;
    }

    public async Task<ApiResponse> PrenoteCreate(CustomerResult c, PaymentBase.PaymentEntries paymentEntry, double amount, string desc, string addenda)
    {
        var requestData = new
        {
            customer = c,
            description = desc,
            amount,
            standard_entry_class = paymentEntry.ToString().Replace("_", " "),
            addenda
        };

        var ret = await APIPost<_RefundResult>("prenotes", CreateHttpContent(requestData));
        return ret;
    }

    public async Task<ApiResponse> PrenoteUpdate(string prenoteId, Payment.PaymentStatuses status)
    {
        var ret = await APIPatch<_PaymentResult>($"prenotes/{prenoteId}", CreateHttpContent(new { status = status.ToString().Replace("_", " ") }));
        return ret;
    }
}
