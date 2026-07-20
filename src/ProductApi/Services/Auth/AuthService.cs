using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProductApi.Common.Exceptions;
using ProductApi.Common.Utilities;
using ProductApi.Dtos.Auth;
using ProductApi.Models;
using ProductApi.Repositories;

namespace ProductApi.Services.Auth;

public class AuthService : IAuthService
{
    private const string UserEmailConstraint = "IX_Users_Email";

    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
    }

    public async Task<User> RegisterAsync(
        RegisterDto request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existingUser = await _userRepository.GetByEmailAsync(
            normalizedEmail,
            cancellationToken);

        if (existingUser is not null)
        {
            throw new ConflictException(
                "An account with this email already exists.");
        }

        var user = new User
        {
            FullName = TextNormalizer.NormalizeWhitespace(request.FullName),
            Email = normalizedEmail,
            Role = UserRoles.User,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordService.HashPassword(
            user,
            request.Password);

        try
        {
            await _userRepository.AddAsync(user, cancellationToken);
        }
        catch (DbUpdateException exception)
            when (IsDuplicateEmailException(exception))
        {
            throw new ConflictException(
                "An account with this email already exists.");
        }

        return user;
    }

    private static bool IsDuplicateEmailException(
        DbUpdateException exception)
    {
        return exception.InnerException is PostgresException postgresException
            && postgresException.SqlState
                == PostgresErrorCodes.UniqueViolation
            && postgresException.ConstraintName
                == UserEmailConstraint;
    }
}
