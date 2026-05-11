using Microsoft.EntityFrameworkCore;
using ProshoreHouseBroker.Application.DTOs;
using ProshoreHouseBroker.Application.Interfaces;
using ProshoreHouseBroker.Domain.Entities;
using ProshoreHouseBroker.Infrastructure.Persistence;

namespace ProshoreHouseBroker.Infrastructure.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PropertyRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(PropertyListing property, CancellationToken cancellationToken = default)
    {
        await _dbContext.PropertyListings.AddAsync(property, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PropertyListing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PropertyListings
            .Include(x => x.Images)
            .Include(x => x.Broker)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PagedResult<PropertyListing>> SearchAsync(PropertySearchCriteriaDto criteria, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.PropertyListings
            .AsNoTracking()
            .Include(x => x.Images)
            .Include(x => x.Broker)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(criteria.Location))
        {
            query = query.Where(x => x.Location.Contains(criteria.Location));
        }

        if (criteria.MinPrice.HasValue)
        {
            query = query.Where(x => x.Price >= criteria.MinPrice.Value);
        }

        if (criteria.MaxPrice.HasValue)
        {
            query = query.Where(x => x.Price <= criteria.MaxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.PropertyType))
        {
            query = query.Where(x => x.PropertyType == criteria.PropertyType);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<PropertyListing>
        {
            TotalCount = totalCount,
            Items = items
        };
    }

    public async Task UpdateAsync(PropertyListing property, CancellationToken cancellationToken = default)
    {
        var existingImages = _dbContext.PropertyImages.Where(x => x.PropertyListingId == property.Id);
        _dbContext.PropertyImages.RemoveRange(existingImages);
        _dbContext.PropertyListings.Update(property);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(PropertyListing property, CancellationToken cancellationToken = default)
    {
        _dbContext.PropertyListings.Remove(property);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
