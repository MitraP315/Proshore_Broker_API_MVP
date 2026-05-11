using ProshoreHouseBroker.Application.DTOs;

namespace ProshoreHouseBroker.Application.Interfaces;

public interface IPropertyService
{
    Task<Guid> CreateAsync(CreateListingRequestDto request, Guid brokerId, CancellationToken cancellationToken = default);

    Task<PropertySearchResultDto> SearchAsync(PropertySearchRequestDto request, CancellationToken cancellationToken = default);

    Task<PropertyResponseDto?> GetByIdAsync(Guid id, Guid? requesterId, CancellationToken cancellationToken = default);

    Task UpdateAsync(Guid id, CreateListingRequestDto request, Guid brokerId, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, Guid brokerId, CancellationToken cancellationToken = default);
}
