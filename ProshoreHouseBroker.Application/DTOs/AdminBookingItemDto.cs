namespace ProshoreHouseBroker.Application.DTOs;

public class AdminBookingItemDto
{
    public Guid BookingId { get; set; }

    public Guid PropertyListingId { get; set; }

    public string PropertyTitle { get; set; } = string.Empty;

    public Guid BrokerId { get; set; }

    public string BrokerName { get; set; } = string.Empty;

    public Guid HouseSeekerId { get; set; }

    public string HouseSeekerName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public decimal PropertyCommissionAmount { get; set; }

    public decimal AdminCommissionAmount { get; set; }

    public decimal BrokerNetCommissionAmount { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? ConfirmedAtUtc { get; set; }
}
