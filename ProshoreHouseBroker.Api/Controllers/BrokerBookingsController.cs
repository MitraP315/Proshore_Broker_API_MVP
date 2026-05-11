using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProshoreHouseBroker.Api.Extensions;
using ProshoreHouseBroker.Application.Interfaces;
using ProshoreHouseBroker.Domain.Enums;

namespace ProshoreHouseBroker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = UserRoles.Broker)]
public class BrokerBookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BrokerBookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings(CancellationToken cancellationToken)
    {
        var bookings = await _bookingService.GetBrokerBookingsAsync(User.GetRequiredUserId(), cancellationToken);
        return Ok(bookings);
    }

    [HttpPost("{bookingId:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid bookingId, CancellationToken cancellationToken)
    {
        await _bookingService.ConfirmAsync(bookingId, User.GetRequiredUserId(), cancellationToken);
        return Ok(new { message = "Booking confirmed and commission split applied." });
    }
}
