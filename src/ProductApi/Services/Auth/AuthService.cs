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
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordService passwordService,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
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

    public async Task<AuthResponseDto> LoginAsync(
        LoginDto request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(
            normalizedEmail,
            cancellationToken);

        if (user is null || !_passwordService.VerifyPassword(
                user,
                user.PasswordHash,
                request.Password))
        {
            throw new UnauthorizedException(
                "Email or password is incorrect.");
        }

        var response = _tokenService.CreateAccessToken(user);
        var refreshTokenResult = _refreshTokenService.CreateRefreshToken();

        var refreshToken = new RefreshToken
        {
            TokenHash = refreshTokenResult.TokenHash,
            UserId = user.Id,
            ExpiresAt = refreshTokenResult.ExpiresAt,
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.AddAsync(
            refreshToken,
            cancellationToken);

        response.RefreshToken = refreshTokenResult.Token;
        response.RefreshTokenExpiresAt = refreshTokenResult.ExpiresAt;

        return response;
    }

    public async Task<AuthResponseDto> RefreshAsync(
        RefreshTokenDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new UnauthorizedException(
                "Refresh token is invalid.");
        }

        var tokenHash = _refreshTokenService.HashToken(
            request.RefreshToken);
        var storedRefreshToken = await _refreshTokenRepository.GetByHashAsync(
            tokenHash,
            cancellationToken);

        if (storedRefreshToken is null)
        {
            throw new UnauthorizedException(
                "Refresh token is invalid.");
        }

        if (storedRefreshToken.RevokedAt is not null)
        {
            throw new UnauthorizedException(
                "Refresh token has been revoked.");
        }

        var now = DateTime.UtcNow;

        if (storedRefreshToken.ExpiresAt <= now)
        {
            throw new UnauthorizedException(
                "Refresh token has expired.");
        }

        var newRefreshTokenResult =
            _refreshTokenService.CreateRefreshToken();

        storedRefreshToken.RevokedAt = now;
        storedRefreshToken.ReplacedByTokenHash =
            newRefreshTokenResult.TokenHash;

        var newRefreshToken = new RefreshToken
        {
            TokenHash = newRefreshTokenResult.TokenHash,
            UserId = storedRefreshToken.UserId,
            ExpiresAt = newRefreshTokenResult.ExpiresAt,
            CreatedAt = now
        };

        await _refreshTokenRepository.AddAsync(
            newRefreshToken,
            cancellationToken);

        var response = _tokenService.CreateAccessToken(
            storedRefreshToken.User);
        response.RefreshToken = newRefreshTokenResult.Token;
        response.RefreshTokenExpiresAt =
            newRefreshTokenResult.ExpiresAt;

        return response;
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
