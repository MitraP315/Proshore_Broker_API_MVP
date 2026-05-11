using ProshoreHouseBroker.Application.DTOs;
using ProshoreHouseBroker.Application.Exceptions;
using ProshoreHouseBroker.Application.Interfaces;
using ProshoreHouseBroker.Domain.Entities;
using ProshoreHouseBroker.Domain.Enums;

namespace ProshoreHouseBroker.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICommissionRuleRepository _commissionRuleRepository;

    public BookingService(
        IBookingRepository bookingRepository,
        IPropertyRepository propertyRepository,
        ICommissionRuleRepository commissionRuleRepository)
    {
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
        _commissionRuleRepository = commissionRuleRepository;
    }

    public async Task<Guid> CreateAsync(BookingRequestDto request, Guid houseSeekerId, CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyListingId, cancellationToken)
            ?? throw new NotFoundException($"Listing '{request.PropertyListingId}' was not found.");

        if (await _bookingRepository.ExistsAsync(request.PropertyListingId, houseSeekerId, cancellationToken))
        {
            throw new ForbiddenException("You have already booked this property.");
        }

        var booking = new PropertyBooking
        {
            Id = Guid.NewGuid(),
            PropertyListingId = request.PropertyListingId,
            HouseSeekerId = houseSeekerId,
            Notes = request.Notes.Trim(),
            Status = BookingStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _bookingRepository.AddAsync(booking, cancellationToken);
        return booking.Id;
    }

    public async Task<IReadOnlyCollection<BookingResponseDto>> GetMyBookingsAsync(Guid houseSeekerId, CancellationToken cancellationToken = default)
    {
        var bookings = await _bookingRepository.GetByHouseSeekerAsync(houseSeekerId, cancellationToken);
        return bookings.Select(ToHouseSeekerResponse).ToArray();
    }

    public async Task<IReadOnlyCollection<BrokerBookingResponseDto>> GetBrokerBookingsAsync(Guid brokerId, CancellationToken cancellationToken = default)
    {
        var bookings = await _bookingRepository.GetByBrokerAsync(brokerId, cancellationToken);

        return bookings.Select(x => new BrokerBookingResponseDto
        {
            BookingId = x.Id,
            PropertyListingId = x.PropertyListingId,
            PropertyTitle = x.PropertyListing.Title,
            HouseSeekerName = x.HouseSeeker.FullName,
            HouseSeekerEmail = x.HouseSeeker.Email ?? string.Empty,
            HouseSeekerPhoneNumber = x.HouseSeeker.PhoneNumber ?? string.Empty,
            Notes = x.Notes,
            Status = x.Status,
            AdminCommissionAmount = x.AdminCommissionAmount,
            BrokerNetCommissionAmount = x.BrokerNetCommissionAmount,
            CreatedAtUtc = x.CreatedAtUtc,
            ConfirmedAtUtc = x.ConfirmedAtUtc
        }).ToArray();
    }

    public async Task ConfirmAsync(Guid bookingId, Guid brokerId, CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken)
            ?? throw new NotFoundException($"Booking '{bookingId}' was not found.");

        if (booking.PropertyListing.BrokerId != brokerId)
        {
            throw new ForbiddenException("You can only confirm bookings for your own properties.");
        }

        if (booking.Status == BookingStatus.Confirmed)
        {
            throw new ForbiddenException("This booking has already been confirmed.");
        }

        var commissionRule = await _commissionRuleRepository.GetApplicableRuleAsync(booking.PropertyListing.Price, cancellationToken);
        var adminSharePercentage = commissionRule?.AdminSharePercentage ?? 0m;
        var adminAmount = Math.Round(booking.PropertyListing.CommissionAmount * adminSharePercentage / 100m, 2, MidpointRounding.AwayFromZero);

        booking.AdminCommissionAmount = adminAmount;
        booking.BrokerNetCommissionAmount = booking.PropertyListing.CommissionAmount - adminAmount;
        booking.Status = BookingStatus.Confirmed;
        booking.ConfirmedAtUtc = DateTime.UtcNow;

        await _bookingRepository.UpdateAsync(booking, cancellationToken);
    }

    private static BookingResponseDto ToHouseSeekerResponse(PropertyBooking booking)
    {
        return new BookingResponseDto
        {
            Id = booking.Id,
            PropertyListingId = booking.PropertyListingId,
            PropertyTitle = booking.PropertyListing.Title,
            BrokerName = booking.PropertyListing.Broker.FullName,
            BrokerPhoneNumber = booking.PropertyListing.Broker.PhoneNumber ?? string.Empty,
            Notes = booking.Notes,
            Status = booking.Status,
            AdminCommissionAmount = booking.AdminCommissionAmount,
            BrokerNetCommissionAmount = booking.BrokerNetCommissionAmount,
            CreatedAtUtc = booking.CreatedAtUtc,
            ConfirmedAtUtc = booking.ConfirmedAtUtc
        };
    }
}
