using Microsoft.AspNetCore.Http;

namespace ProshoreHouseBroker.Api.Services;

public class LocalImageStorageService : IImageStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalImageStorageService(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
    {
        _environment = environment;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IReadOnlyCollection<string>> SaveAsync(IEnumerable<IFormFile> files, CancellationToken cancellationToken = default)
    {
        var savedUrls = new List<string>();
        var rootPath = _environment.WebRootPath;

        if (string.IsNullOrWhiteSpace(rootPath))
        {
            rootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
        }

        var uploadFolder = Path.Combine(rootPath, "uploads", "properties");
        Directory.CreateDirectory(uploadFolder);

        foreach (var file in files.Where(x => x.Length > 0))
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(uploadFolder, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);

            savedUrls.Add(BuildPublicUrl(fileName));
        }

        return savedUrls;
    }

    private string BuildPublicUrl(string fileName)
    {
        var request = _httpContextAccessor.HttpContext?.Request;

        if (request is null)
        {
            return $"/uploads/properties/{fileName}";
        }

        return $"{request.Scheme}://{request.Host}/uploads/properties/{fileName}";
    }
}
