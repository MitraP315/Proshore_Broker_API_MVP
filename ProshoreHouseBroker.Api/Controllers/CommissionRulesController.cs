using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProshoreHouseBroker.Application.DTOs;
using ProshoreHouseBroker.Application.Interfaces;
using ProshoreHouseBroker.Domain.Entities;
using ProshoreHouseBroker.Domain.Enums;

namespace ProshoreHouseBroker.Api.Controllers
{

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = UserRoles.Admin)]
public class CommissionRulesController : ControllerBase
{
    private readonly ICommissionRuleRepository _repository;

    public CommissionRulesController(
        ICommissionRuleRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rules = await _repository.GetAllAsync();
        return Ok(rules);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var rule = await _repository.GetByIdAsync(id);

        if (rule is null)
            return NotFound();

        return Ok(rule);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCommissionRuleDto request)
    {
        var rule = new CommissionRule
        {
            Id = Guid.NewGuid(),
            MinAmount = request.MinAmount,
            MaxAmount = request.MaxAmount,
            Percentage = request.Percentage,
            AdminSharePercentage = request.AdminSharePercentage,
            IsActive = true
        };

        await _repository.AddAsync(rule);
        await _repository.SaveChangesAsync();

        return Ok(rule);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateCommissionRuleDto request)
    {
        var rule = await _repository.GetByIdAsync(id);

        if (rule is null)
            return NotFound();

        rule.MinAmount = request.MinAmount;
        rule.MaxAmount = request.MaxAmount;
        rule.Percentage = request.Percentage;
        rule.AdminSharePercentage =
            request.AdminSharePercentage;
        rule.IsActive = request.IsActive;

        await _repository.UpdateAsync(rule);
        await _repository.SaveChangesAsync();

        return Ok(rule);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var rule = await _repository.GetByIdAsync(id);

        if (rule is null)
            return NotFound();

        await _repository.DeleteAsync(rule);
        await _repository.SaveChangesAsync();

        return Ok(new
        {
            message = "Commission rule deleted."
        });
    }
}

}