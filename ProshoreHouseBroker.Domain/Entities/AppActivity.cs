namespace ProshoreHouseBroker.Domain.Entities;

public class AppActivity
{
    public Guid Id { get; set; }

    public string Path { get; set; } = string.Empty;

    public string Method { get; set; } = string.Empty;

    public int StatusCode { get; set; }

    public string? EndpointName { get; set; }

    public string? UserId { get; set; }

    public string? DeviceId { get; set; }

    public string? IpAddress { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
