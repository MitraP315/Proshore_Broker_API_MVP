namespace ProshoreHouseBroker.Application.DTOs;

public class BrokerCommissionListingDto
{
    public Guid PropertyId { get; set; }

    public string Title { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public decimal CommissionAmount { get; set; }

    public decimal AdminCommissionShareAmount { get; set; }

    public decimal BrokerNetCommissionAmount { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
