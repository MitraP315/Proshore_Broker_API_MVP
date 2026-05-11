namespace ProshoreHouseBroker.Domain.Entities;

public class ErrorInfo
{
    public Guid Id { get; set; }

    public string Message { get; set; } = string.Empty;

    public string ExceptionType { get; set; } = string.Empty;

    public string? StackTrace { get; set; }

    public string Path { get; set; } = string.Empty;

    public string Method { get; set; } = string.Empty;

    public string? UserId { get; set; }

    public string? DeviceId { get; set; }

    public string? IpAddress { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
