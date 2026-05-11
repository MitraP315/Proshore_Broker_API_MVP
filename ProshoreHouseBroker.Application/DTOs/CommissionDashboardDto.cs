namespace ProshoreHouseBroker.Application.DTOs;

public class CommissionDashboardDto
{
    public decimal GrandTotalCommission { get; set; }

    public decimal GrandTotalAdminCommissionShare { get; set; }

    public decimal GrandTotalBrokerNetCommission { get; set; }

    public int BrokerCount { get; set; }

    public IReadOnlyCollection<BrokerCommissionReportDto> Brokers { get; set; } = Array.Empty<BrokerCommissionReportDto>();
}
