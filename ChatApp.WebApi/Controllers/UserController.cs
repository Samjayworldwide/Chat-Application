using System.Diagnostics.CodeAnalysis;
using ChatApp.Application.commons;
using ChatApp.Application.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/user")]
[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("upload-avatar")]
    [RequestSizeLimit(5 * 1024 * 1024)] // Limit file size to 5 MB
    [Produces(typeof(Result<string>))]
    public async Task<IActionResult> UploadAvatar([FromForm] IFormFile avatar, CancellationToken cancellationToken)
    {
        var result = await _userService.UploadAvatarAsync(User, avatar, cancellationToken);

        if (!result.IsSuccessful)
            return BadRequest(result);

        return Ok(result);
    }
}