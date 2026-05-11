using Microsoft.EntityFrameworkCore;
using ProshoreHouseBroker.Application.Interfaces;
using ProshoreHouseBroker.Domain.Entities;
using ProshoreHouseBroker.Infrastructure.Persistence;

namespace ProshoreHouseBroker.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BookingRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(PropertyBooking booking, CancellationToken cancellationToken = default)
    {
        await _dbContext.PropertyBookings.AddAsync(booking, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid propertyListingId, Guid houseSeekerId, CancellationToken cancellationToken = default)
    {
        return _dbContext.PropertyBookings.AnyAsync(
            x => x.PropertyListingId == propertyListingId && x.HouseSeekerId == houseSeekerId,
            cancellationToken);
    }

    public async Task<PropertyBooking?> GetByIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PropertyBookings
            .Include(x => x.PropertyListing)
            .ThenInclude(x => x.Broker)
            .Include(x => x.HouseSeeker)
            .FirstOrDefaultAsync(x => x.Id == bookingId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<PropertyBooking>> GetByHouseSeekerAsync(Guid houseSeekerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PropertyBookings
            .AsNoTracking()
            .Include(x => x.PropertyListing)
            .ThenInclude(x => x.Broker)
            .Where(x => x.HouseSeekerId == houseSeekerId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<PropertyBooking>> GetByBrokerAsync(Guid brokerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PropertyBookings
            .AsNoTracking()
            .Include(x => x.PropertyListing)
            .ThenInclude(x => x.Broker)
            .Include(x => x.HouseSeeker)
            .Where(x => x.PropertyListing.BrokerId == brokerId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(PropertyBooking booking, CancellationToken cancellationToken = default)
    {
        _dbContext.PropertyBookings.Update(booking);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
