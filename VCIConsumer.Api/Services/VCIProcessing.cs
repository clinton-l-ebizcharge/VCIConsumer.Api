using System.Dynamic;
using System.Text;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Services;


public class VCIProcessing : ServiceBase
{
    // public string MerchantId { get; set; }

    public VCIProcessing(string clientId, string secretKey) : base(clientId, secretKey)
    {
        // this.MerchantId = merchantId;
    }

    #region Payment
    public async Task<IResult> PaymentList(Payment.PaymentStatuses paymentStatus, DateTime fromDate, DateTime todate)
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

        var rs = await APIGet<Payment[]>(strb.ToString());
        return rs;
    }

    public async Task<IResult> PaymentDetail(string paymentId)
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
        // paymentId = paymentId.StartsWith("PMT_", StringComparison.OrdinalIgnoreCase) ? paymentId : $"PMT_{paymentId}";


        var rs = await APIGet<Payment>($"{urlPath}/{paymentId}");
        return rs;


    }
    public async Task<IResult> PaymentCreate(_Customer c, PaymentBase.PaymentEntries entryClass, double amount, _Check check, string description)
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
        IResult ret = await APIPost<_PaymentResult>("payments", httpContent);

        //    string Log = $"{  JsonConvert.SerializeObject(this.AccessToken)}\n\n {JsonConvert.SerializeObject(requestData)}\n\n${JsonConvert.SerializeObject(ret)}";


        return ret;
    }
    public async Task<IResult> PaymentVoid(string paymentId)
    {

        var rs = await APIPatch<_PaymentResult>($"payments/{paymentId}", CreateHttpContent(new { status = Payment.PaymentStatuses.VOID.ToString().Replace("_", " ") }));
        return rs;
    }
    public async Task<IResult> PaymentUpdate(string paymentId, Payment.PaymentStatuses status)
    {

        var rs = await APIPatch<_PaymentResult>($"payments/{paymentId}", CreateHttpContent(new { status = status.ToString().Replace("_", " ") }));
        return rs;
    }
    #endregion
    #region Refund
    public async Task<IResult> RefundList(Payment.PaymentStatuses paymentStatus, DateTime fromDate, DateTime todate)
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
        strb.Append("refunds?");
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
    public async Task<IResult> RefundDetail(string refundId)
    {

        refundId = refundId.StartsWith("RFN_", StringComparison.OrdinalIgnoreCase) ? refundId : $"RFN_{refundId}";
        var rs = await APIGet<Payment>($"refunds/{refundId}");
        return rs;
    }
    public async Task<IResult> RefundCreate(string paymentId, double amount)
    {
        var requestData = new
        {
            original_payment_uuid = paymentId,
            amount,

        };

        var rs = await APIPost<_RefundResult>("refunds", CreateHttpContent(requestData));
        return rs;
    }
    public async Task<IResult> RefundUpdate(string refundId, Payment.PaymentStatuses status)
    {
        var ret = await APIPatch<_RefundResult>($"refunds/{refundId}", CreateHttpContent(new { status = status.ToString().Replace("_", " ") }));
        return ret;
    }
    #endregion
    #region Payout


    public async Task<IResult> PayoutList(Payment.PaymentStatuses paymentStatus, DateTime fromDate, DateTime todate)
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
        strb.Append("payouts?");
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
    public async Task<IResult> PayoutDetail(string payoutId)
    {

        payoutId = payoutId.StartsWith("POT_", StringComparison.OrdinalIgnoreCase) ? payoutId : $"POT_{payoutId}";
        var rs = await APIGet<Payment>($"payout/{payoutId}");
        return rs;
    }
    public async Task<IResult> PayoutCreate(_Customer c, PaymentBase.PaymentEntries paymentEntry, double amount, string desc, string addenda)
    {
        var requestData = new
        {
            customer = c,
            description = desc,
            amount,
            standard_entry_class = paymentEntry.ToString().Replace("_", " "),
            addenda

        };

        var ret = await APIPost<_PaymentResult>("payouts", CreateHttpContent(requestData));
        return ret;
    }
    public async Task<IResult> PayoutUpdate(string potId, Payment.PaymentStatuses status)
    {


        var ret = await APIPatch<_RefundResult>($"payouts/{potId}", CreateHttpContent(new { status = status.ToString().Replace("_", " ") }));
        return ret;
    }


    #endregion
    #region Prenote
    public async Task<IResult> PrenoteList(Payment.PaymentStatuses paymentStatus, DateTime fromDate, DateTime todate)
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
    public async Task<IResult> PrenoteDetail(string prenoteId)
    {

        prenoteId = prenoteId.StartsWith("NTE_", StringComparison.OrdinalIgnoreCase) ? prenoteId : $"NTE_{prenoteId}";
        var rs = await APIGet<Payment>($"prenotes/{prenoteId}");
        return rs;
    }
    public async Task<IResult> PrenoteCreate(CustomerResult c, PaymentBase.PaymentEntries paymentEntry, double amount, string desc, string addenda)
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
    public async Task<IResult> PrenoteUpdate(string prenoteId, Payment.PaymentStatuses status)
    {


        var ret = await APIPatch<_PaymentResult>($"prenotes/{prenoteId}", CreateHttpContent(new { status = status.ToString().Replace("_", " ") }));
        return ret;
    }

    #endregion

}
