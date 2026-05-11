namespace ProshoreHouseBroker.Domain.Entities;

public class CommissionRule
{
    public Guid Id { get; set; }

    public decimal MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }

    public decimal Percentage { get; set; }

    public decimal AdminSharePercentage { get; set; }

    public bool IsActive { get; set; } = true;
}
