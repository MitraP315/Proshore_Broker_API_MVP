using ProshoreHouseBroker.Application.DTOs;
using ProshoreHouseBroker.Application.Exceptions;
using ProshoreHouseBroker.Application.Interfaces;
using ProshoreHouseBroker.Application.Services;
using ProshoreHouseBroker.Application.Validators;
using ProshoreHouseBroker.Domain.Entities;

namespace ProshoreHouseBroker.Tests;

public class PropertyServiceTests
{
    [Fact]
    public async Task CreateAsync_SetsCommissionAndPersistsListing()
    {
        var repository = new FakePropertyRepository();
        var service = new PropertyService(repository, new StubCommissionService(120000m), new CreateListingRequestValidator());
        var brokerId = Guid.NewGuid();

        var id = await service.CreateAsync(new CreateListingRequestDto
        {
            Title = "City Apartment",
            Description = "Two bedroom apartment",
            Price = 8000000m,
            Location = "Kathmandu",
            PropertyType = "Apartment",
            Features = "Parking, Balcony",
            ImageUrls = ["https://example.com/1.jpg", "https://example.com/2.jpg"]
        }, brokerId);

        var created = await repository.GetByIdAsync(id);

        Assert.NotNull(created);
        Assert.Equal(120000m, created!.CommissionAmount);
        Assert.Equal(brokerId, created.BrokerId);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsWhenListingOwnedByAnotherBroker()
    {
        var repository = new FakePropertyRepository();
        var listing = repository.SeedListing();
        var service = new PropertyService(repository, new StubCommissionService(100000m), new CreateListingRequestValidator());

        await Assert.ThrowsAsync<ForbiddenException>(() => service.UpdateAsync(
            listing.Id,
            new CreateListingRequestDto
            {
                Title = "Updated",
                Description = "Updated description",
                Price = 6000000m,
                Location = "Pokhara",
                PropertyType = "House",
                Features = "Garden",
                ImageUrls = ["https://example.com/updated.jpg"]
            },
            Guid.NewGuid()));
    }

    private sealed class StubCommissionService : ICommissionService
    {
        private readonly decimal _amount;

        public StubCommissionService(decimal amount)
        {
            _amount = amount;
        }

        public Task<decimal> CalculateCommissionAsync(decimal price, CancellationToken cancellationToken = default)
            => Task.FromResult(_amount);
    }

    private sealed class FakePropertyRepository : IPropertyRepository
    {
        private readonly List<PropertyListing> _items = [];

        public Task AddAsync(PropertyListing property, CancellationToken cancellationToken = default)
        {
            property.Broker = new AppUser
            {
                Id = property.BrokerId,
                FullName = "Owner Broker",
                Email = "owner@example.com",
                PhoneNumber = "9811111111"
            };

            _items.Add(property);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(PropertyListing property, CancellationToken cancellationToken = default)
        {
            _items.Remove(property);
            return Task.CompletedTask;
        }

        public Task<PropertyListing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

        public Task<PagedResult<PropertyListing>> SearchAsync(PropertySearchCriteriaDto criteria, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResult<PropertyListing> { TotalCount = _items.Count, Items = _items });

        public Task UpdateAsync(PropertyListing property, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public PropertyListing SeedListing()
        {
            var brokerId = Guid.NewGuid();
            var listing = new PropertyListing
            {
                Id = Guid.NewGuid(),
                Title = "Original",
                Description = "Original",
                Price = 5000000m,
                Location = "Kathmandu",
                PropertyType = "House",
                Features = "Parking",
                BrokerId = brokerId,
                Broker = new AppUser
                {
                    Id = brokerId,
                    FullName = "Owner Broker",
                    Email = "owner@example.com",
                    PhoneNumber = "9811111111"
                },
                Images = [new PropertyImage { Id = Guid.NewGuid(), ImageUrl = "https://example.com/home.jpg" }],
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            _items.Add(listing);
            return listing;
        }
    }
}
