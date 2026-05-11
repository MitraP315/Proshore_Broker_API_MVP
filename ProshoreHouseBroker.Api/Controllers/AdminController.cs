using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProshoreHouseBroker.Api.Filters;
using ProshoreHouseBroker.Application.Interfaces;
using ProshoreHouseBroker.Domain.Enums;

namespace ProshoreHouseBroker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = UserRoles.Admin)]
[RequireVerifiedEmail]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] string? role, CancellationToken cancellationToken)
    {
        var users = await _adminService.GetUsersAsync(role, cancellationToken);
        return Ok(users);
    }

    [HttpGet("commissions")]
    public async Task<IActionResult> GetCommissionDashboard(CancellationToken cancellationToken)
    {
        var dashboard = await _adminService.GetCommissionDashboardAsync(cancellationToken);
        return Ok(dashboard);
    }

    [HttpGet("commissions/{brokerId:guid}")]
    public async Task<IActionResult> GetBrokerCommissionReport(Guid brokerId, CancellationToken cancellationToken)
    {
        var report = await _adminService.GetBrokerCommissionReportAsync(brokerId, cancellationToken);
        return report is null ? NotFound() : Ok(report);
    }

    [HttpGet("bookings/summary")]
    public async Task<IActionResult> GetBookingSummary(CancellationToken cancellationToken)
    {
        var summary = await _adminService.GetBookingSummaryAsync(cancellationToken);
        return Ok(summary);
    }

    [HttpGet("bookings")]
    public async Task<IActionResult> GetBookings([FromQuery] string? status, [FromQuery] Guid? brokerId, CancellationToken cancellationToken)
    {
        var bookings = await _adminService.GetBookingsAsync(status, brokerId, cancellationToken);
        return Ok(bookings);
    }
}

