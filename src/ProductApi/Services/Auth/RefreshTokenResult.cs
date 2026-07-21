namespace ProductApi.Services.Auth;

public sealed class RefreshTokenResult
{
    public string Token { get; init; } = string.Empty;

    public string TokenHash { get; init; } = string.Empty;

    public DateTime ExpiresAt { get; init; }
}
