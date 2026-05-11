namespace ProshoreHouseBroker.Application.DTOs;

public class BookingResponseDto
{
    public Guid Id { get; set; }

    public Guid PropertyListingId { get; set; }

    public string PropertyTitle { get; set; } = string.Empty;

    public string BrokerName { get; set; } = string.Empty;

    public string BrokerPhoneNumber { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public decimal AdminCommissionAmount { get; set; }

    public decimal BrokerNetCommissionAmount { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? ConfirmedAtUtc { get; set; }
}
