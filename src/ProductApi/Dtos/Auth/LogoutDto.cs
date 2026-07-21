using System.ComponentModel.DataAnnotations;

namespace ProductApi.Dtos.Auth;

public class LogoutDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
