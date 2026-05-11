using System.ComponentModel.DataAnnotations;

namespace ProshoreHouseBroker.Application.DTOs;

public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }
}
