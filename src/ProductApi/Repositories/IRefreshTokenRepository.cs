using ProductApi.Models;

namespace ProductApi.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default);
}
