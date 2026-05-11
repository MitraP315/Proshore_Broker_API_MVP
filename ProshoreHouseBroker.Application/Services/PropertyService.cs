using FluentValidation;
using ProshoreHouseBroker.Application.DTOs;
using ProshoreHouseBroker.Application.Exceptions;
using ProshoreHouseBroker.Application.Interfaces;
using ProshoreHouseBroker.Domain.Entities;

namespace ProshoreHouseBroker.Application.Services;

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICommissionService _commissionService;
    private readonly IValidator<CreateListingRequestDto> _validator;

    public PropertyService(
        IPropertyRepository propertyRepository,
        ICommissionService commissionService,
        IValidator<CreateListingRequestDto> validator)
    {
        _propertyRepository = propertyRepository;
        _commissionService = commissionService;
        _validator = validator;
    }

    public async Task<Guid> CreateAsync(CreateListingRequestDto request, Guid brokerId, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var now = DateTime.UtcNow;
        var commissionAmount = await _commissionService.CalculateCommissionAsync(request.Price, cancellationToken);
        var listing = BuildListing(request, brokerId, commissionAmount, now);

        await _propertyRepository.AddAsync(listing, cancellationToken);
        return listing.Id;
    }

    public async Task<PropertyResponseDto?> GetByIdAsync(Guid id, Guid? requesterId, CancellationToken cancellationToken = default)
    {
        var listing = await _propertyRepository.GetByIdAsync(id, cancellationToken);
        return listing is null ? null : ToResponse(listing, requesterId);
    }

    public async Task<PropertySearchResultDto> SearchAsync(PropertySearchRequestDto request, CancellationToken cancellationToken = default)
    {
        var criteria = new PropertySearchCriteriaDto
        {
            Location = request.Location?.Trim(),
            MinPrice = request.MinPrice,
            MaxPrice = request.MaxPrice,
            PropertyType = request.PropertyType?.Trim(),
            Page = Math.Max(1, request.Page),
            PageSize = Math.Clamp(request.PageSize, 1, 100)
        };

        var result = await _propertyRepository.SearchAsync(criteria, cancellationToken);

        return new PropertySearchResultDto
        {
            TotalCount = result.TotalCount,
            Items = result.Items.Select(x => ToResponse(x, null)).ToArray()
        };
    }

    public async Task UpdateAsync(Guid id, CreateListingRequestDto request, Guid brokerId, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var listing = await _propertyRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Listing '{id}' was not found.");

        EnsureOwner(listing, brokerId);

        listing.Title = request.Title.Trim();
        listing.Description = request.Description.Trim();
        listing.Price = request.Price;
        listing.Location = request.Location.Trim();
        listing.PropertyType = request.PropertyType.Trim();
        listing.Features = request.Features.Trim();
        listing.CommissionAmount = await _commissionService.CalculateCommissionAsync(request.Price, cancellationToken);
        listing.UpdatedAtUtc = DateTime.UtcNow;
        listing.Images = request.ImageUrls
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(x => new PropertyImage
            {
                Id = Guid.NewGuid(),
                PropertyListingId = listing.Id,
                ImageUrl = x.Trim()
            })
            .ToList();

        await _propertyRepository.UpdateAsync(listing, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, Guid brokerId, CancellationToken cancellationToken = default)
    {
        var listing = await _propertyRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Listing '{id}' was not found.");

        EnsureOwner(listing, brokerId);
        await _propertyRepository.DeleteAsync(listing, cancellationToken);
    }

    private static void EnsureOwner(PropertyListing listing, Guid brokerId)
    {
        if (listing.BrokerId != brokerId)
        {
            throw new ForbiddenException("You can only modify your own listings.");
        }
    }

    private static PropertyListing BuildListing(CreateListingRequestDto request, Guid brokerId, decimal commissionAmount, DateTime now)
    {
        var listingId = Guid.NewGuid();

        return new PropertyListing
        {
            Id = listingId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Price = request.Price,
            Location = request.Location.Trim(),
            PropertyType = request.PropertyType.Trim(),
            Features = request.Features.Trim(),
            BrokerId = brokerId,
            CommissionAmount = commissionAmount,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
            Images = request.ImageUrls
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(x => new PropertyImage
                {
                    Id = Guid.NewGuid(),
                    PropertyListingId = listingId,
                    ImageUrl = x.Trim()
                })
                .ToList()
        };
    }

    private static PropertyResponseDto ToResponse(PropertyListing listing, Guid? requesterId)
    {
        var canSeeCommission = requesterId.HasValue && requesterId == listing.BrokerId;

        return new PropertyResponseDto
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            Price = listing.Price,
            Location = listing.Location,
            PropertyType = listing.PropertyType,
            Features = listing.Features,
            Images = listing.Images.Select(x => x.ImageUrl).ToArray(),
            BrokerName = listing.Broker.FullName,
            BrokerEmail = listing.Broker.Email ?? string.Empty,
            BrokerPhoneNumber = listing.Broker.PhoneNumber ?? string.Empty,
            CommissionAmount = canSeeCommission ? listing.CommissionAmount : null,
            CreatedAtUtc = listing.CreatedAtUtc
        };
    }
}
