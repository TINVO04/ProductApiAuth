using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Common.Responses;
using ProductApi.Dtos.Users;
using ProductApi.Models;
using ProductApi.Services.Users;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<IReadOnlyList<UserResponseDto>>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UserResponseDto>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);

        return Ok(new ApiResponse<IReadOnlyList<UserResponseDto>>
        {
            Success = true,
            Message = "Users retrieved successfully.",
            Data = users
        });
    }
}
