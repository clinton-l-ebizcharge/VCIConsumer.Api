namespace VCIConsumer.Api.Models.Responses;

public class TokenResponse
{
    public string AccessToken { get; set; } = null!; 
    public int ExpiresIn { get; set; } 
}
