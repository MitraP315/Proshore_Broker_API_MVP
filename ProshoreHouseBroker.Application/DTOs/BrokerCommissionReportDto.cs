namespace ProshoreHouseBroker.Application.DTOs;

public class BrokerCommissionReportDto
{
    public Guid BrokerId { get; set; }

    public string BrokerName { get; set; } = string.Empty;

    public string BrokerEmail { get; set; } = string.Empty;

    public int TotalListings { get; set; }

    public decimal TotalCommissionAmount { get; set; }

    public decimal TotalAdminCommissionShareAmount { get; set; }

    public decimal TotalBrokerNetCommissionAmount { get; set; }

    public IReadOnlyCollection<BrokerCommissionListingDto> Listings { get; set; } = Array.Empty<BrokerCommissionListingDto>();
}
