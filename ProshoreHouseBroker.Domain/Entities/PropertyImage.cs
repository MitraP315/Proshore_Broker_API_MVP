namespace ProshoreHouseBroker.Domain.Entities;

public class PropertyImage
{
    public Guid Id { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public Guid PropertyListingId { get; set; }

    public PropertyListing PropertyListing { get; set; } = null!;
}
