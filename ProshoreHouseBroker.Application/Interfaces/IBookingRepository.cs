using ProshoreHouseBroker.Domain.Entities;

namespace ProshoreHouseBroker.Application.Interfaces;

public interface IBookingRepository
{
    Task AddAsync(PropertyBooking booking, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid propertyListingId, Guid houseSeekerId, CancellationToken cancellationToken = default);

    Task<PropertyBooking?> GetByIdAsync(Guid bookingId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PropertyBooking>> GetByHouseSeekerAsync(Guid houseSeekerId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PropertyBooking>> GetByBrokerAsync(Guid brokerId, CancellationToken cancellationToken = default);

    Task UpdateAsync(PropertyBooking booking, CancellationToken cancellationToken = default);
}
