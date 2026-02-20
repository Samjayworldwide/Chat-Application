using System.Diagnostics.CodeAnalysis;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ChatApp.Infrastructure.BlobStorageInfrastructure.Interfaces;
using ChatApp.Infrastructure.configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChatApp.Infrastructure.BlobStorageInfrastructure.Implementation;

[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(IOptions<BlobStorageSettings> options, BlobServiceClient blobServiceClient,
        ILogger<BlobStorageService> logger)
    {
        var blobStorageSettings = options.Value ?? throw new ArgumentNullException(nameof(options));

        _containerClient = blobServiceClient.GetBlobContainerClient(blobStorageSettings.ContainerName);

        _logger = logger;
    }

    public async Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        try
        {
            await _containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

            var blobName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            var blobClient = _containerClient.GetBlobClient(blobName);

            await using var stream = file.OpenReadStream();

            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType },
                cancellationToken: cancellationToken);

            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while uploading file {}", ex.Message);

            return string.Empty;
        }
    }
}