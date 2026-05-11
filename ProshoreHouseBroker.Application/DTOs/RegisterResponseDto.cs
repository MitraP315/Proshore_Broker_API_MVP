namespace ProshoreHouseBroker.Application.DTOs;

public class RegisterResponseDto
{
    public string UserId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public bool RequiresEmailConfirmation { get; set; }

    public string? EmailConfirmationToken { get; set; }

    public string Message { get; set; } = string.Empty;
}
