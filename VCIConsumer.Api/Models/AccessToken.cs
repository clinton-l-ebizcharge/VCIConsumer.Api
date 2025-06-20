namespace VCIConsumer.Api.Models;

public class AccessToken
{
    public required string Token { get; set; }
    public required string TokenType { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
    public bool IsExpired => string.IsNullOrEmpty(Token) || ExpiresAt < DateTime.Now.ToUniversalTime();
    public bool IsValid => !string.IsNullOrEmpty(Token) && !IsExpired; 
}
