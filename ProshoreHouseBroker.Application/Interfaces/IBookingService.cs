using ProshoreHouseBroker.Application.DTOs;

namespace ProshoreHouseBroker.Application.Interfaces;

public interface IBookingService
{
    Task<Guid> CreateAsync(BookingRequestDto request, Guid houseSeekerId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<BookingResponseDto>> GetMyBookingsAsync(Guid houseSeekerId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<BrokerBookingResponseDto>> GetBrokerBookingsAsync(Guid brokerId, CancellationToken cancellationToken = default);

    Task ConfirmAsync(Guid bookingId, Guid brokerId, CancellationToken cancellationToken = default);
}
