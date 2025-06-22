namespace VCIConsumer.Api.Models;

public class AccessToken
{
    public required string Token { get; set; }
    public required string TokenType { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsExpired(ITimeProvider clock) => string.IsNullOrEmpty(Token) || ExpiresAt < clock.UtcNow;
}
