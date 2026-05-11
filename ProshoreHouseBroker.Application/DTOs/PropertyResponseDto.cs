namespace ProshoreHouseBroker.Application.DTOs;

public class PropertyResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Location { get; set; } = string.Empty;

    public string PropertyType { get; set; } = string.Empty;

    public string Features { get; set; } = string.Empty;

    public IReadOnlyCollection<string> Images { get; set; } = Array.Empty<string>();

    public string BrokerName { get; set; } = string.Empty;

    public string BrokerEmail { get; set; } = string.Empty;

    public string BrokerPhoneNumber { get; set; } = string.Empty;

    public decimal? CommissionAmount { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
