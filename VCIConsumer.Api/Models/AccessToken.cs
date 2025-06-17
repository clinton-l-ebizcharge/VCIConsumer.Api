namespace VCIConsumer.Api.Models;

public class AccessToken
{
    public required string Token { get; set; } = string.Empty;
    public required string TokenType { get; set; } = "Bearer";
    public int ExpiresIn { get; set; } = 3600;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsExpired => string.IsNullOrEmpty(Token) || CreatedAt.AddSeconds(ExpiresIn) < DateTime.Now;
    public bool IsValid => !string.IsNullOrEmpty(Token) && !IsExpired; 
}
