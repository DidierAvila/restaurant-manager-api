namespace RestaurantManager.Application.DTOs.AccessControl;

public class AccountDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Type { get; set; } = null!;
    public string Provider { get; set; } = null!;
    public string ProviderAccountId { get; set; } = null!;
    public long? ExpiresAt { get; set; }
    public string? Scope { get; set; }
    public string? TokenType { get; set; }
}

public class CreateAccountDto
{
    public Guid UserId { get; set; }
    public string Type { get; set; } = null!;
    public string Provider { get; set; } = null!;
    public string ProviderAccountId { get; set; } = null!;
    public string? RefreshToken { get; set; }
    public string? AccessToken { get; set; }
    public long? ExpiresAt { get; set; }
    public string? Scope { get; set; }
    public string? TokenType { get; set; }
}
