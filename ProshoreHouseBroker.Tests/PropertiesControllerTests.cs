using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProshoreHouseBroker.Api.Controllers;
using ProshoreHouseBroker.Api.Models;
using ProshoreHouseBroker.Api.Services;
using ProshoreHouseBroker.Application.DTOs;
using ProshoreHouseBroker.Application.Interfaces;
using System.Security.Claims;

namespace ProshoreHouseBroker.Tests;

public class PropertiesControllerTests
{
    [Fact]
    public async Task Search_ReturnsOkResult()
    {
        var controller = new PropertiesController(new FakePropertyService(), new FakeImageStorageService());

        var result = await controller.Search(new PropertySearchRequestDto { Location = "Kathmandu" }, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<PropertySearchResultDto>(ok.Value);
        Assert.Single(payload.Items);
    }

    [Fact]
    public async Task GetById_ReturnsNotFoundWhenServiceReturnsNull()
    {
        var controller = new PropertiesController(new FakePropertyService(), new FakeImageStorageService());

        var result = await controller.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var controller = new PropertiesController(new FakePropertyService(), new FakeImageStorageService())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.NameIdentifier, Guid.Parse("ef298f3d-2212-486f-86c6-f4e92b0d0f38").ToString()),
                        new Claim(ClaimTypes.Role, "Broker")
                    ], "Test"))
                }
            }
        };

        var result = await controller.Create(new CreatePropertyFormRequest
        {
            Title = "Fresh Listing",
            Description = "Fresh property",
            Price = 6500000m,
            Location = "Kathmandu",
            PropertyType = "Apartment",
            Features = "Lift, Parking",
            ImageUrls = ["https://example.com/home.jpg"]
        }, CancellationToken.None);

        Assert.IsType<CreatedAtActionResult>(result.Result);
    }

    private sealed class FakePropertyService : IPropertyService
    {
        public Task<Guid> CreateAsync(CreateListingRequestDto request, Guid brokerId, CancellationToken cancellationToken = default)
            => Task.FromResult(Guid.NewGuid());

        public Task DeleteAsync(Guid id, Guid brokerId, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<PropertyResponseDto?> GetByIdAsync(Guid id, Guid? requesterId, CancellationToken cancellationToken = default)
            => Task.FromResult<PropertyResponseDto?>(null);

        public Task<PropertySearchResultDto> SearchAsync(PropertySearchRequestDto request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PropertySearchResultDto
            {
                TotalCount = 1,
                Items =
                [
                    new PropertyResponseDto
                    {
                        Id = Guid.NewGuid(),
                        Title = "Sample",
                        Description = "Sample listing",
                        Price = 5000000m,
                        Location = request.Location ?? "Kathmandu",
                        PropertyType = "House",
                        Features = "Garden",
                        Images = ["https://example.com/sample.jpg"],
                        BrokerName = "Broker Name",
                        BrokerEmail = "broker@example.com",
                        BrokerPhoneNumber = "9800000000"
                    }
                ]
            });
        }

        public Task UpdateAsync(Guid id, CreateListingRequestDto request, Guid brokerId, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class FakeImageStorageService : IImageStorageService
    {
        public Task<IReadOnlyCollection<string>> SaveAsync(IEnumerable<IFormFile> files, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<string>>(Array.Empty<string>());
    }
}
