namespace VCIConsumer.Api.Models;

public class AccessToken
{
    public required string Token { get; set; }
    public required string TokenType { get; set; }
    public required DateTime ExpiresIn { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsExpired => string.IsNullOrEmpty(Token) || ExpiresIn < DateTime.Now;
    public bool IsValid => !string.IsNullOrEmpty(Token) && !IsExpired; 
}
