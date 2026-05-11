using ProshoreHouseBroker.Application.DTOs;

namespace ProshoreHouseBroker.Application.Interfaces;

public interface IAdminService
{
    Task<IReadOnlyCollection<UserSummaryDto>> GetUsersAsync(string? role, CancellationToken cancellationToken = default);

    Task<CommissionDashboardDto> GetCommissionDashboardAsync(CancellationToken cancellationToken = default);

    Task<BrokerCommissionReportDto?> GetBrokerCommissionReportAsync(Guid brokerId, CancellationToken cancellationToken = default);

    Task<AdminBookingSummaryDto> GetBookingSummaryAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AdminBookingItemDto>> GetBookingsAsync(string? status, Guid? brokerId, CancellationToken cancellationToken = default);
}
