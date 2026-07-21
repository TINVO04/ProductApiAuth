namespace ProductApi.Services.Auth;

public interface IRefreshTokenService
{
    RefreshTokenResult CreateRefreshToken();

    string HashToken(string token);
}
