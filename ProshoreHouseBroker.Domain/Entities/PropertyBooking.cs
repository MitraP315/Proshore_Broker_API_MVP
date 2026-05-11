namespace ProshoreHouseBroker.Domain.Entities;

public class PropertyBooking
{
    public Guid Id { get; set; }

    public Guid PropertyListingId { get; set; }

    public PropertyListing PropertyListing { get; set; } = null!;

    public Guid HouseSeekerId { get; set; }

    public AppUser HouseSeeker { get; set; } = null!;

    public string Notes { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public decimal AdminCommissionAmount { get; set; }

    public decimal BrokerNetCommissionAmount { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? ConfirmedAtUtc { get; set; }
}
