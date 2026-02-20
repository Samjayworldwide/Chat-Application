using System.Security.Claims;
using ChatApp.Application.commons;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.interfaces;

public interface IUserService
{
    Task<Result<string>> UploadAvatarAsync(ClaimsPrincipal claimsPrincipal, IFormFile file,
        CancellationToken cancellationToken = default);
}