using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using ProductApi.Options;

namespace ProductApi.Services.Auth;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private const int TokenByteLength = 64;

    private readonly JwtOptions _jwtOptions;

    public RefreshTokenService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public RefreshTokenResult CreateRefreshToken()
    {
        var token = Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(TokenByteLength));

        return new RefreshTokenResult
        {
            Token = token,
            TokenHash = HashToken(token),
            ExpiresAt = DateTime.UtcNow.AddDays(
                _jwtOptions.RefreshTokenDays)
        };
    }

    public string HashToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }
}
