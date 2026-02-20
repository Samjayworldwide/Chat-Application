using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using ChatApp.Application.commons;
using ChatApp.Application.interfaces;
using ChatApp.Infrastructure.BlobStorageInfrastructure.Interfaces;
using ChatApp.Infrastructure.repositories.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ChatApp.Application.implementations;

[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    private readonly IBlobStorageService _blobStorageService;

    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, IBlobStorageService blobStorageService,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;

        _blobStorageService = blobStorageService;

        _logger = logger;
    }

    public async Task<Result<string>> UploadAvatarAsync(ClaimsPrincipal claimsPrincipal, IFormFile file,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Result<string>.Failure("User ID not found in claims.");

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return Result<string>.Failure("User not found.");

            var avatarUrl = await _blobStorageService.UploadAsync(file, cancellationToken);

            if (string.IsNullOrEmpty(avatarUrl))
                return Result<string>.Failure("Failed to upload avatar.");

            user.AvatarUrl = avatarUrl;

            await _userRepository.UpdateUserAsync(user);

            return Result<string>.Success("Avatar uploaded successfully.", avatarUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while uploading avatar: {}", ex.Message);

            return Result<string>.Failure("An error occurred while uploading avatar.");
        }
    }
}