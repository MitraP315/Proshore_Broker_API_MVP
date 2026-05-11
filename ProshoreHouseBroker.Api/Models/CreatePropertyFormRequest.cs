using Microsoft.AspNetCore.Http;

namespace ProshoreHouseBroker.Api.Models;

public class CreatePropertyFormRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Location { get; set; } = string.Empty;

    public string PropertyType { get; set; } = string.Empty;

    public string Features { get; set; } = string.Empty;

    public List<string> ImageUrls { get; set; } = [];

    public List<IFormFile> Images { get; set; } = [];
}
