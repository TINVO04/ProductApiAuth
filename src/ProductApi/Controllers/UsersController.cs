using Microsoft.AspNetCore.Mvc;
using ProductApi.Common.Responses;
using ProductApi.Dtos.Users;
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

    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<IReadOnlyList<UserResponseDto>>),
        StatusCodes.Status200OK)]
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
