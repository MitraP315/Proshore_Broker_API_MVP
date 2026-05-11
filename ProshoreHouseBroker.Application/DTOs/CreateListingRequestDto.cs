namespace ProshoreHouseBroker.Application.DTOs;

public class CreateListingRequestDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Location { get; set; } = string.Empty;

    public string PropertyType { get; set; } = string.Empty;

    public string Features { get; set; } = string.Empty;

    public List<string> ImageUrls { get; set; } = [];
}
