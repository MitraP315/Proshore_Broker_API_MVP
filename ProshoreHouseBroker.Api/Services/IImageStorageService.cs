using Microsoft.AspNetCore.Http;

namespace ProshoreHouseBroker.Api.Services;

public interface IImageStorageService
{
    Task<IReadOnlyCollection<string>> SaveAsync(IEnumerable<IFormFile> files, CancellationToken cancellationToken = default);
}
