namespace ProshoreHouseBroker.Application.DTOs;

public class UserSummaryDto
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public int ListingCount { get; set; }

    public int BookingCount { get; set; }
}
