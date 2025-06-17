namespace VCIConsumer.Api.Services;

public class APIResponseResult<TResponse>
{
    public HttpResponseMessage MM { get; set; }
    public bool OK => MM.IsSuccessStatusCode;
    private TResponse? _DataObject; // Marked as nullable to address CS8618
    public TResponse DataObject
    {
        get
        {
            if (_DataObject == null)
            {
                var js = GetRawData().GetAwaiter().GetResult();
                _DataObject = Newtonsoft.Json.JsonConvert.DeserializeObject<TResponse>(js)
                              ?? throw new InvalidOperationException("Deserialization returned null.");
            }
            return _DataObject;
        }
    }
    public async Task<string> GetRawData()
    {
        return await MM.Content.ReadAsStringAsync();
    }

    // Updated to use primary constructor to address IDE0290
    public APIResponseResult(HttpResponseMessage mm) => MM = mm;
}
