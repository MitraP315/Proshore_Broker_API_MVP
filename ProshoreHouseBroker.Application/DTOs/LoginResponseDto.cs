namespace ProshoreHouseBroker.Application.DTOs;

public class LoginResponseDto
{
    public string? Token { get; set; }

    public DateTime? ExpiresAtUtc { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public bool RequiresMfa { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? DebugMfaCode { get; set; }
}
