using ProshoreHouseBroker.Domain.Entities;

namespace ProshoreHouseBroker.Application.Interfaces;

public interface ICommissionRuleRepository
{
    Task<IReadOnlyCollection<CommissionRule>> GetActiveRulesAsync(CancellationToken cancellationToken = default);

    Task<CommissionRule?> GetApplicableRuleAsync(decimal price, CancellationToken cancellationToken = default); 
    Task<List<CommissionRule>> GetAllAsync();

    Task<CommissionRule?> GetByIdAsync(Guid id);

    Task AddAsync(CommissionRule rule);

    Task UpdateAsync(CommissionRule rule);

    Task DeleteAsync(CommissionRule rule);

    Task SaveChangesAsync();
}
