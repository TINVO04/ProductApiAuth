using ProductApi.Dtos.Auth;
using ProductApi.Models;

namespace ProductApi.Services.Auth;

public interface IAuthService
{
    Task<User> RegisterAsync(
        RegisterDto request,
        CancellationToken cancellationToken = default);

    Task<AuthResponseDto> LoginAsync(
        LoginDto request,
        CancellationToken cancellationToken = default);

    Task<AuthResponseDto> RefreshAsync(
        RefreshTokenDto request,
        CancellationToken cancellationToken = default);

    Task LogoutAsync(
        LogoutDto request,
        CancellationToken cancellationToken = default);
}
