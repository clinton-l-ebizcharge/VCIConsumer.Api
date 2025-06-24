using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Enums;

public partial class Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StandardEntryClass
    {
        PPD,
        CCD,
        BOC,
        WEB,
        TEL,
        POP
    }
}
