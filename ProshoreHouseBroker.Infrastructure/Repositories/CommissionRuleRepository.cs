using Microsoft.EntityFrameworkCore;
using ProshoreHouseBroker.Application.Interfaces;
using ProshoreHouseBroker.Domain.Entities;
using ProshoreHouseBroker.Infrastructure.Persistence;

namespace ProshoreHouseBroker.Infrastructure.Repositories;

public class CommissionRuleRepository : ICommissionRuleRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CommissionRuleRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<CommissionRule>> GetActiveRulesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.CommissionRules
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.MinAmount)
            .ToListAsync(cancellationToken);
    }

    public Task<CommissionRule?> GetApplicableRuleAsync(decimal price, CancellationToken cancellationToken = default)
    {
        return _dbContext.CommissionRules
            .AsNoTracking()
            .Where(x => x.IsActive && price >= x.MinAmount && (!x.MaxAmount.HasValue || price <= x.MaxAmount.Value))
            .OrderBy(x => x.MinAmount)
            .FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<List<CommissionRule>> GetAllAsync()
    {
        return await _dbContext.CommissionRules
            .OrderBy(x => x.MinAmount)
            .ToListAsync();
    }

    public async Task<CommissionRule?> GetByIdAsync(Guid id)
    {
        return await _dbContext.CommissionRules
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(CommissionRule rule)
    {
        await _dbContext.CommissionRules.AddAsync(rule);
    }

    public Task UpdateAsync(CommissionRule rule)
    {
        _dbContext.CommissionRules.Update(rule);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(CommissionRule rule)
    {
        _dbContext.CommissionRules.Remove(rule);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
