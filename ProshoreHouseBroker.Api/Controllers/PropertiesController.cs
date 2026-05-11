using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProshoreHouseBroker.Api.Extensions;
using ProshoreHouseBroker.Api.Models;
using ProshoreHouseBroker.Api.Services;
using ProshoreHouseBroker.Application.DTOs;
using ProshoreHouseBroker.Application.Interfaces;
using ProshoreHouseBroker.Domain.Enums;

namespace ProshoreHouseBroker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;
    private readonly IImageStorageService _imageStorageService;

    public PropertiesController(IPropertyService propertyService, IImageStorageService imageStorageService)
    {
        _propertyService = propertyService;
        _imageStorageService = imageStorageService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PropertySearchResultDto>> Search([FromQuery] PropertySearchRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _propertyService.SearchAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<PropertyResponseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var principal = HttpContext?.User;
        Guid? requesterId = principal?.Identity?.IsAuthenticated == true ? principal.GetUserId() : null;
        var result = await _propertyService.GetByIdAsync(id, requesterId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Broker)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Guid>> Create([FromForm] CreatePropertyFormRequest request, CancellationToken cancellationToken)
    {
        var id = await _propertyService.CreateAsync(await MapToDtoAsync(request, cancellationToken), User.GetRequiredUserId(), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = UserRoles.Broker)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(Guid id, [FromForm] CreatePropertyFormRequest request, CancellationToken cancellationToken)
    {
        await _propertyService.UpdateAsync(id, await MapToDtoAsync(request, cancellationToken), User.GetRequiredUserId(), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = UserRoles.Broker)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _propertyService.DeleteAsync(id, User.GetRequiredUserId(), cancellationToken);
        return NoContent();
    }

    private async Task<CreateListingRequestDto> MapToDtoAsync(CreatePropertyFormRequest request, CancellationToken cancellationToken)
    {
        var uploadedImageUrls = await _imageStorageService.SaveAsync(request.Images, cancellationToken);

        return new CreateListingRequestDto
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            Location = request.Location,
            PropertyType = request.PropertyType,
            Features = request.Features,
            ImageUrls = request.ImageUrls
                .Concat(uploadedImageUrls)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
        };
    }
}
