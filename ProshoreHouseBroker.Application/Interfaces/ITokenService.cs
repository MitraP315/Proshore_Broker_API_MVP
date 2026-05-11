using ProshoreHouseBroker.Domain.Entities;

namespace ProshoreHouseBroker.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(AppUser user);
}
