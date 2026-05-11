using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProshoreHouseBroker.Application.DTOs;
using ProshoreHouseBroker.Application.Interfaces;
using ProshoreHouseBroker.Infrastructure.Persistence;

namespace ProshoreHouseBroker.Infrastructure.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AdminRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BrokerCommissionReportDto?> GetBrokerCommissionReportAsync(Guid brokerId, CancellationToken cancellationToken = default)
    {
        var broker = await _dbContext.Users
            .AsNoTracking()
            .Include(x => x.Listings)
            .ThenInclude(x => x.Bookings)
            .FirstOrDefaultAsync(x => x.Id == brokerId, cancellationToken);

        return broker is null ? null : MapBrokerReport(broker);
    }

    public async Task<IReadOnlyCollection<BrokerCommissionReportDto>> GetBrokerCommissionReportsAsync(CancellationToken cancellationToken = default)
    {
        var brokers = await _dbContext.Users
            .AsNoTracking()
            .Include(x => x.Listings)
            .ThenInclude(x => x.Bookings)
            .Where(x => x.Listings.Any())
            .ToListAsync(cancellationToken);

        return brokers
            .Select(MapBrokerReport)
            .OrderByDescending(x => x.TotalCommissionAmount)
            .ToArray();
    }

    public async Task<IReadOnlyCollection<UserSummaryDto>> GetUsersAsync(string? role, CancellationToken cancellationToken = default)
    {
        var rows = await (
            from user in _dbContext.Users.AsNoTracking()
            join userRole in _dbContext.UserRoles.AsNoTracking() on user.Id equals userRole.UserId
            join roleEntity in _dbContext.Roles.AsNoTracking() on userRole.RoleId equals roleEntity.Id
            where string.IsNullOrWhiteSpace(role) || roleEntity.Name == role
            select new UserSummaryDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Role = roleEntity.Name ?? string.Empty,
                ListingCount = user.Listings.Count,
                BookingCount = user.Bookings.Count
            })
            .OrderBy(x => x.Role)
            .ThenBy(x => x.FullName)
            .ToListAsync(cancellationToken);

        return rows;
    }

    public async Task<AdminBookingSummaryDto> GetBookingSummaryAsync(CancellationToken cancellationToken = default)
    {
        var bookings = await _dbContext.PropertyBookings
            .AsNoTracking()
            .Include(x => x.PropertyListing)
            .ToListAsync(cancellationToken);

        return new AdminBookingSummaryDto
        {
            TotalBookings = bookings.Count,
            PendingBookings = bookings.Count(x => x.Status == "Pending"),
            ConfirmedBookings = bookings.Count(x => x.Status == "Confirmed"),
            CancelledBookings = bookings.Count(x => x.Status == "Cancelled"),
            TotalCommissionAmount = bookings
                .Where(x => x.Status == "Confirmed")
                .Sum(x => x.PropertyListing.CommissionAmount),
            TotalAdminCommissionAmount = bookings.Sum(x => x.AdminCommissionAmount),
            TotalBrokerNetCommissionAmount = bookings.Sum(x => x.BrokerNetCommissionAmount)
        };
    }

    public async Task<IReadOnlyCollection<AdminBookingItemDto>> GetBookingsAsync(string? status, Guid? brokerId, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.PropertyBookings
            .AsNoTracking()
            .Include(x => x.PropertyListing)
            .ThenInclude(x => x.Broker)
            .Include(x => x.HouseSeeker)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status);
        }

        if (brokerId.HasValue)
        {
            query = query.Where(x => x.PropertyListing.BrokerId == brokerId.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new AdminBookingItemDto
            {
                BookingId = x.Id,
                PropertyListingId = x.PropertyListingId,
                PropertyTitle = x.PropertyListing.Title,
                BrokerId = x.PropertyListing.BrokerId,
                BrokerName = x.PropertyListing.Broker.FullName,
                HouseSeekerId = x.HouseSeekerId,
                HouseSeekerName = x.HouseSeeker.FullName,
                Status = x.Status,
                PropertyCommissionAmount = x.PropertyListing.CommissionAmount,
                AdminCommissionAmount = x.AdminCommissionAmount,
                BrokerNetCommissionAmount = x.BrokerNetCommissionAmount,
                CreatedAtUtc = x.CreatedAtUtc,
                ConfirmedAtUtc = x.ConfirmedAtUtc
            })
            .ToListAsync(cancellationToken);
    }

    private static BrokerCommissionReportDto MapBrokerReport(Domain.Entities.AppUser user)
    {
        return new BrokerCommissionReportDto
        {
            BrokerId = user.Id,
            BrokerName = user.FullName,
            BrokerEmail = user.Email ?? string.Empty,
            TotalListings = user.Listings.Count,
            TotalCommissionAmount = user.Listings.Sum(listing => listing.CommissionAmount),
            TotalAdminCommissionShareAmount = user.Listings.SelectMany(listing => listing.Bookings).Sum(booking => booking.AdminCommissionAmount),
            TotalBrokerNetCommissionAmount = user.Listings.SelectMany(listing => listing.Bookings).Sum(booking => booking.BrokerNetCommissionAmount),
            Listings = user.Listings
                .OrderByDescending(listing => listing.CreatedAtUtc)
                .Select(listing => new BrokerCommissionListingDto
                {
                    PropertyId = listing.Id,
                    Title = listing.Title,
                    Price = listing.Price,
                    CommissionAmount = listing.CommissionAmount,
                    AdminCommissionShareAmount = listing.Bookings.Sum(booking => booking.AdminCommissionAmount),
                    BrokerNetCommissionAmount = listing.Bookings.Sum(booking => booking.BrokerNetCommissionAmount),
                    CreatedAtUtc = listing.CreatedAtUtc
                })
                .ToArray()
        };
    }
}
