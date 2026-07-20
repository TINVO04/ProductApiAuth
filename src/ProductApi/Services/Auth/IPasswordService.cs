using ProductApi.Models;

namespace ProductApi.Services.Auth;

public interface IPasswordService
{
    string HashPassword(User user, string password);

    bool VerifyPassword(User user, string passwordHash, string password);
}
