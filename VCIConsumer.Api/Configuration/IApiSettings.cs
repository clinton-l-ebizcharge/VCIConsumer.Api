namespace VCIConsumer.Api.Configuration;

public interface IApiSettings
{
    string BaseUrl { get; set; }
    string? ClientId { get; set; }
    string ClientName { get; set; }
    string? ClientSecret { get; set; }
    string VeriCheckVersion { get; set; }
}