namespace ProshoreHouseBroker.Application.DTOs;

public class PagedResult<T>
{
    public int TotalCount { get; set; }

    public IReadOnlyCollection<T> Items { get; set; } = Array.Empty<T>();
}
