using Microsoft.AspNetCore.Identity;
using ProductApi.Models;

namespace ProductApi.Services.Auth;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(
        User user,
        string passwordHash,
        string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            user,
            passwordHash,
            password);

        return result != PasswordVerificationResult.Failed;
    }
}
