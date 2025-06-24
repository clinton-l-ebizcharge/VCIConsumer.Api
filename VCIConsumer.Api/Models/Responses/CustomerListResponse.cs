using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses;

public class CustomerListResponse
{
    [JsonPropertyName("customers")]
    public List<CustomerListCustomerDetailResponse> Customers { get; set; }
}
