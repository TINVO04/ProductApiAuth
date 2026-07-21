using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Common.Responses;
using ProductApi.Dtos.Auth;
using ProductApi.Services.Auth;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<object>>> Register(
        [FromBody] RegisterDto request,
        CancellationToken cancellationToken)
    {
        var user = await _authService.RegisterAsync(
            request,
            cancellationToken);

        var response = new ApiResponse<object>
        {
            Success = true,
            Message = "User registered successfully.",
            Data = new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Role,
                user.CreatedAt
            }
        };

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("login")]
    [ProducesResponseType(
        typeof(ApiResponse<AuthResponseDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(
        [FromBody] LoginDto request,
        CancellationToken cancellationToken)
    {
        var authResult = await _authService.LoginAsync(
            request,
            cancellationToken);

        var response = new ApiResponse<AuthResponseDto>
        {
            Success = true,
            Message = "Login successful.",
            Data = authResult
        };

        return Ok(response);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(
        typeof(ApiResponse<AuthResponseDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh(
        [FromBody] RefreshTokenDto request,
        CancellationToken cancellationToken)
    {
        var authResult = await _authService.RefreshAsync(
            request,
            cancellationToken);

        var response = new ApiResponse<AuthResponseDto>
        {
            Success = true,
            Message = "Token refreshed successfully.",
            Data = authResult
        };

        return Ok(response);
    }

    [HttpPost("logout")]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> Logout(
        [FromBody] LogoutDto request,
        CancellationToken cancellationToken)
    {
        await _authService.LogoutAsync(
            request,
            cancellationToken);

        var response = new ApiResponse<object>
        {
            Success = true,
            Message = "Logout successful."
        };

        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status401Unauthorized)]
    public ActionResult<ApiResponse<object>> GetCurrentUser()
    {
        var response = new ApiResponse<object>
        {
            Success = true,
            Message = "Current user retrieved successfully.",
            Data = new
            {
                UserId = User.FindFirst("userId")?.Value,
                Email = User.FindFirst("email")?.Value,
                Role = User.FindFirst("role")?.Value
            }
        };

        return Ok(response);
    }
}
