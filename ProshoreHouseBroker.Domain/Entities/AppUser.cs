using Microsoft.AspNetCore.Identity;

namespace ProshoreHouseBroker.Domain.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;

    public ICollection<PropertyListing> Listings { get; set; } = new List<PropertyListing>();

    public ICollection<PropertyBooking> Bookings { get; set; } = new List<PropertyBooking>();
}
