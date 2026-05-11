namespace ProshoreHouseBroker.Application.DTOs;

public class BookingRequestDto
{
    public Guid PropertyListingId { get; set; }

    public string Notes { get; set; } = string.Empty;
}
