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
}
