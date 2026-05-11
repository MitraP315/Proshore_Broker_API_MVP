using System.ComponentModel.DataAnnotations;

namespace ProshoreHouseBroker.Application.DTOs;

public class PropertySearchRequestDto
{
    public string? Location { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public string? PropertyType { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
}
