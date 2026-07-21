using ProductApi.Dtos.Users;

namespace ProductApi.Services.Users;

public interface IUserService
{
    Task<IReadOnlyList<UserResponseDto>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
