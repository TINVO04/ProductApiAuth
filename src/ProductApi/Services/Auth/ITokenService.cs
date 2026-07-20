using ProductApi.Dtos.Auth;
using ProductApi.Models;

namespace ProductApi.Services.Auth;

public interface ITokenService
{
    AuthResponseDto CreateAccessToken(User user);
}
