using ProshoreHouseBroker.Application.Interfaces;

namespace ProshoreHouseBroker.Application.Services;

public class CommissionService : ICommissionService
{
    private readonly ICommissionRuleRepository _commissionRuleRepository;

    public CommissionService(ICommissionRuleRepository commissionRuleRepository)
    {
        _commissionRuleRepository = commissionRuleRepository;
    }

    public async Task<decimal> CalculateCommissionAsync(decimal price, CancellationToken cancellationToken = default)
    {
        var rules = await _commissionRuleRepository.GetActiveRulesAsync(cancellationToken);

        var rule = rules
            .OrderBy(x => x.MinAmount)
            .FirstOrDefault(x => price >= x.MinAmount && (!x.MaxAmount.HasValue || price <= x.MaxAmount.Value));

        if (rule is null)
        {
            return 0m;
        }

        return Math.Round(price * rule.Percentage / 100m, 2, MidpointRounding.AwayFromZero);
    }
}
