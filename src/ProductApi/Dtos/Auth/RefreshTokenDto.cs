using System.ComponentModel.DataAnnotations;

namespace ProductApi.Dtos.Auth;

public class RefreshTokenDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
