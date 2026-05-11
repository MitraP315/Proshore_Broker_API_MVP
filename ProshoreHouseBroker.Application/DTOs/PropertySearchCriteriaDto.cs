namespace ProshoreHouseBroker.Application.DTOs;

public class PropertySearchCriteriaDto
{
    public string? Location { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public string? PropertyType { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
