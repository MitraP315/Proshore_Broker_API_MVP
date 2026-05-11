namespace ProshoreHouseBroker.Application.DTOs;

public class AdminBookingSummaryDto
{
    public int TotalBookings { get; set; }

    public int PendingBookings { get; set; }

    public int ConfirmedBookings { get; set; }

    public int CancelledBookings { get; set; }

    public decimal TotalCommissionAmount { get; set; }

    public decimal TotalAdminCommissionAmount { get; set; }

    public decimal TotalBrokerNetCommissionAmount { get; set; }
}
