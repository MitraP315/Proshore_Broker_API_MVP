using ProshoreHouseBroker.Application.DTOs;
using ProshoreHouseBroker.Domain.Entities;

namespace ProshoreHouseBroker.Application.Interfaces;

public interface IPropertyRepository
{
    Task AddAsync(PropertyListing property, CancellationToken cancellationToken = default);

    Task<PropertyListing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<PropertyListing>> SearchAsync(PropertySearchCriteriaDto criteria, CancellationToken cancellationToken = default);

    Task UpdateAsync(PropertyListing property, CancellationToken cancellationToken = default);

    Task DeleteAsync(PropertyListing property, CancellationToken cancellationToken = default);
}
