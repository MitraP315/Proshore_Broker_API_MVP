using System.ComponentModel.DataAnnotations;

namespace ProshoreHouseBroker.Application.DTOs;

public class RequestEmailConfirmationDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
