using ProshoreHouseBroker.Application.DTOs;
using ProshoreHouseBroker.Application.Exceptions;
using ProshoreHouseBroker.Application.Interfaces;
using ProshoreHouseBroker.Application.Services;
using ProshoreHouseBroker.Domain.Entities;
using ProshoreHouseBroker.Domain.Enums;

namespace ProshoreHouseBroker.Tests;

public class BookingServiceTests
{
    [Fact]
    public async Task CreateAsync_CreatesBookingForHouseSeeker()
    {
        var bookingRepository = new FakeBookingRepository();
        var propertyRepository = new FakePropertyRepository();
        var service = new BookingService(bookingRepository, propertyRepository, new FakeCommissionRuleRepository());

        var id = await service.CreateAsync(new BookingRequestDto
        {
            PropertyListingId = propertyRepository.PropertyId,
            Notes = "Please arrange a visit"
        }, Guid.NewGuid());

        Assert.NotEqual(Guid.Empty, id);
        Assert.Single(bookingRepository.Items);
    }

    [Fact]
    public async Task CreateAsync_ThrowsWhenDuplicateBookingExists()
    {
        var bookingRepository = new FakeBookingRepository { Exists = true };
        var propertyRepository = new FakePropertyRepository();
        var service = new BookingService(bookingRepository, propertyRepository, new FakeCommissionRuleRepository());

        await Assert.ThrowsAsync<ForbiddenException>(() => service.CreateAsync(new BookingRequestDto
        {
            PropertyListingId = propertyRepository.PropertyId,
            Notes = "Again"
        }, Guid.NewGuid()));
    }

    [Fact]
    public async Task ConfirmAsync_SplitsCommissionBetweenAdminAndBroker()
    {
        var brokerId = Guid.NewGuid();
        var bookingRepository = new FakeBookingRepository
        {
            Booking = new PropertyBooking
            {
                Id = Guid.NewGuid(),
                PropertyListingId = Guid.NewGuid(),
                PropertyListing = new PropertyListing
                {
                    Id = Guid.NewGuid(),
                    Title = "Booked Listing",
                    Price = 8000000m,
                    CommissionAmount = 140000m,
                    BrokerId = brokerId,
                    Broker = new AppUser { Id = brokerId, FullName = "Broker" }
                },
                HouseSeeker = new AppUser(),
                Status = BookingStatus.Pending
            }
        };

        var service = new BookingService(bookingRepository, new FakePropertyRepository(), new FakeCommissionRuleRepository());

        await service.ConfirmAsync(bookingRepository.Booking!.Id, brokerId);

        Assert.Equal(14000m, bookingRepository.Booking!.AdminCommissionAmount);
        Assert.Equal(126000m, bookingRepository.Booking.BrokerNetCommissionAmount);
        Assert.Equal(BookingStatus.Confirmed, bookingRepository.Booking.Status);
    }

    private sealed class FakeBookingRepository : IBookingRepository
    {
        public bool Exists { get; set; }

        public List<PropertyBooking> Items { get; } = [];

        public PropertyBooking? Booking { get; set; }

        public Task AddAsync(PropertyBooking booking, CancellationToken cancellationToken = default)
        {
            Items.Add(booking);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(Guid propertyListingId, Guid houseSeekerId, CancellationToken cancellationToken = default)
            => Task.FromResult(Exists);

        public Task<PropertyBooking?> GetByIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
            => Task.FromResult(Booking);

        public Task<IReadOnlyCollection<PropertyBooking>> GetByBrokerAsync(Guid brokerId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<PropertyBooking>>(Booking is null ? [] : [Booking]);

        public Task<IReadOnlyCollection<PropertyBooking>> GetByHouseSeekerAsync(Guid houseSeekerId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<PropertyBooking>>(Items);

        public Task UpdateAsync(PropertyBooking booking, CancellationToken cancellationToken = default)
        {
            Booking = booking;
            return Task.CompletedTask;
        }
    }

    private sealed class FakePropertyRepository : IPropertyRepository
    {
        public Guid PropertyId { get; } = Guid.NewGuid();

        public Task AddAsync(PropertyListing property, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(PropertyListing property, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<PropertyListing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var listing = new PropertyListing
            {
                Id = PropertyId,
                Title = "Listing",
                Price = 8000000m,
                CommissionAmount = 140000m,
                BrokerId = Guid.NewGuid(),
                Broker = new AppUser { FullName = "Broker", PhoneNumber = "9800000000" }
            };

            return Task.FromResult<PropertyListing?>(id == PropertyId ? listing : null);
        }

        public Task<PagedResult<PropertyListing>> SearchAsync(PropertySearchCriteriaDto criteria, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResult<PropertyListing>());

        public Task UpdateAsync(PropertyListing property, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeCommissionRuleRepository : ICommissionRuleRepository
    {
        public Task<IReadOnlyCollection<CommissionRule>> GetActiveRulesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<CommissionRule>>([]);

        public Task<CommissionRule?> GetApplicableRuleAsync(decimal price, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<CommissionRule?>(new CommissionRule
            {
                MinAmount = 5000000m,
                MaxAmount = 10000000m,
                Percentage = 1.75m,
                AdminSharePercentage = 10m,
                IsActive = true
            });
        }
    }
}
