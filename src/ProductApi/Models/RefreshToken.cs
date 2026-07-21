namespace ProductApi.Models;

public class RefreshToken
{
    public int Id { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public int UserId { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? ReplacedByTokenHash { get; set; }
}
