namespace ProshoreHouseBroker.Application.Interfaces;

public interface ICommissionService
{
    Task<decimal> CalculateCommissionAsync(decimal price, CancellationToken cancellationToken = default);
}
