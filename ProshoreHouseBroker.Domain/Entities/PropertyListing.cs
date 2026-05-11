namespace ProshoreHouseBroker.Domain.Entities;

public class PropertyListing
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Location { get; set; } = string.Empty;

    public string PropertyType { get; set; } = string.Empty;

    public string Features { get; set; } = string.Empty;

    public decimal CommissionAmount { get; set; }

    public Guid BrokerId { get; set; }

    public AppUser Broker { get; set; } = null!;

    public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();

    public ICollection<PropertyBooking> Bookings { get; set; } = new List<PropertyBooking>();

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
