namespace ProshoreHouseBroker.Application.DTOs;

public class PropertySearchResultDto
{
    public int TotalCount { get; set; }

    public IReadOnlyCollection<PropertyResponseDto> Items { get; set; } = Array.Empty<PropertyResponseDto>();
}
