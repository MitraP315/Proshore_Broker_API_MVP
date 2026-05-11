using System.ComponentModel.DataAnnotations;

namespace ProshoreHouseBroker.Application.DTOs;

public class ConfirmEmailRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;
}
