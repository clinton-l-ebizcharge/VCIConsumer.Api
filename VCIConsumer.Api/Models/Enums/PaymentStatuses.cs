using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentStatus
    {
        ACCEPTED,
        ERROR,
        ORIGINATED,
        SETTLED,
        [EnumMember(Value = "PARTIAL SETTLED")]
        PARTIAL_SETTLED,
        VERIFYING,
        VOID,
        RETURN,
        [EnumMember(Value = "NSF DECLINED")]
        NSF_DECLINED
    }
}
