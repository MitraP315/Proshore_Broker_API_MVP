using ProshoreHouseBroker.Application.DTOs;
using ProshoreHouseBroker.Application.Interfaces;

namespace ProshoreHouseBroker.Application.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;

    public AdminService(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public Task<BrokerCommissionReportDto?> GetBrokerCommissionReportAsync(Guid brokerId, CancellationToken cancellationToken = default)
        => _adminRepository.GetBrokerCommissionReportAsync(brokerId, cancellationToken);

    public async Task<CommissionDashboardDto> GetCommissionDashboardAsync(CancellationToken cancellationToken = default)
    {
        var brokers = await _adminRepository.GetBrokerCommissionReportsAsync(cancellationToken);

        return new CommissionDashboardDto
        {
            BrokerCount = brokers.Count,
            GrandTotalCommission = brokers.Sum(x => x.TotalCommissionAmount),
            GrandTotalAdminCommissionShare = brokers.Sum(x => x.TotalAdminCommissionShareAmount),
            GrandTotalBrokerNetCommission = brokers.Sum(x => x.TotalBrokerNetCommissionAmount),
            Brokers = brokers
        };
    }

    public Task<IReadOnlyCollection<UserSummaryDto>> GetUsersAsync(string? role, CancellationToken cancellationToken = default)
        => _adminRepository.GetUsersAsync(role, cancellationToken);

    public Task<AdminBookingSummaryDto> GetBookingSummaryAsync(CancellationToken cancellationToken = default)
        => _adminRepository.GetBookingSummaryAsync(cancellationToken);

    public Task<IReadOnlyCollection<AdminBookingItemDto>> GetBookingsAsync(string? status, Guid? brokerId, CancellationToken cancellationToken = default)
        => _adminRepository.GetBookingsAsync(status, brokerId, cancellationToken);
}
