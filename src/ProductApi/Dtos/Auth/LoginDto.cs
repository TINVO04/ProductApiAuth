using System.ComponentModel.DataAnnotations;

namespace ProductApi.Dtos.Auth;

public class LoginDto
{
    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
