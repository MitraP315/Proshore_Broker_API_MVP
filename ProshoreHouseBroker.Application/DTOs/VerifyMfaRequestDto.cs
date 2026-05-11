using System.ComponentModel.DataAnnotations;

namespace ProshoreHouseBroker.Application.DTOs;

public class VerifyMfaRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;
}
