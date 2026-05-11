namespace ProshoreHouseBroker.Application.DTOs;

public class BrokerBookingResponseDto
{
    public Guid BookingId { get; set; }

    public Guid PropertyListingId { get; set; }

    public string PropertyTitle { get; set; } = string.Empty;

    public string HouseSeekerName { get; set; } = string.Empty;

    public string HouseSeekerEmail { get; set; } = string.Empty;

    public string HouseSeekerPhoneNumber { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public decimal AdminCommissionAmount { get; set; }

    public decimal BrokerNetCommissionAmount { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? ConfirmedAtUtc { get; set; }
}
