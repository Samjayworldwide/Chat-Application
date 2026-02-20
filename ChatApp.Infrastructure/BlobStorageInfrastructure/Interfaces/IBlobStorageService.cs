using Microsoft.AspNetCore.Http;

namespace ChatApp.Infrastructure.BlobStorageInfrastructure.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken = default);
}