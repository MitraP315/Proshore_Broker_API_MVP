using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProshoreHouseBroker.Api.Extensions;
using ProshoreHouseBroker.Application.DTOs;
using ProshoreHouseBroker.Application.Interfaces;
using ProshoreHouseBroker.Domain.Enums;

namespace ProshoreHouseBroker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = UserRoles.HouseSeeker)]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(BookingRequestDto request, CancellationToken cancellationToken)
    {
        var id = await _bookingService.CreateAsync(request, User.GetRequiredUserId(), cancellationToken);
        return Ok(id);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings(CancellationToken cancellationToken)
    {
        var bookings = await _bookingService.GetMyBookingsAsync(User.GetRequiredUserId(), cancellationToken);
        return Ok(bookings);
    }
}
