using ProshoreHouseBroker.Application.Interfaces;
using ProshoreHouseBroker.Application.Services;
using ProshoreHouseBroker.Domain.Entities;

namespace ProshoreHouseBroker.Tests;

public class CommissionServiceTests
{
    [Theory]
    [InlineData(4000000, 80000)]
    [InlineData(7500000, 131250)]
    [InlineData(15000000, 225000)]
    public async Task CalculateCommissionAsync_UsesConfiguredRule(decimal price, decimal expected)
    {
        var service = new CommissionService(new FakeCommissionRuleRepository());

        var result = await service.CalculateCommissionAsync(price);

        Assert.Equal(expected, result);
    }

    private sealed class FakeCommissionRuleRepository : ICommissionRuleRepository
    {
        public Task<IReadOnlyCollection<CommissionRule>> GetActiveRulesAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<CommissionRule> rules =
            [
                new() { MinAmount = 0, MaxAmount = 4999999.99m, Percentage = 2m, IsActive = true },
                new() { MinAmount = 5000000m, MaxAmount = 10000000m, Percentage = 1.75m, IsActive = true },
                new() { MinAmount = 10000000.01m, MaxAmount = null, Percentage = 1.5m, IsActive = true }
            ];

            return Task.FromResult(rules);
        }

        public Task<CommissionRule?> GetApplicableRuleAsync(decimal price, CancellationToken cancellationToken = default)
        {
            CommissionRule? rule =
                price < 5000000m
                    ? new CommissionRule { MinAmount = 0, MaxAmount = 4999999.99m, Percentage = 2m, AdminSharePercentage = 10m, IsActive = true }
                    : price <= 10000000m
                        ? new CommissionRule { MinAmount = 5000000m, MaxAmount = 10000000m, Percentage = 1.75m, AdminSharePercentage = 10m, IsActive = true }
                        : new CommissionRule { MinAmount = 10000000.01m, MaxAmount = null, Percentage = 1.5m, AdminSharePercentage = 10m, IsActive = true };

            return Task.FromResult<CommissionRule?>(rule);
        }
    }
}
