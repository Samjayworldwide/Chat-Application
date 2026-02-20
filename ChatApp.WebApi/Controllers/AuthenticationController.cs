using System.Diagnostics.CodeAnalysis;
using ChatApp.Application.commands;
using ChatApp.Application.commons;
using ChatApp.Application.interfaces;
using ChatApp.SharedKernel.dtos.response;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    [Produces(typeof(Result<string>))]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationCommand userRegistrationCommand)
    {
        var result = await _authenticationService.RegisterUserAsync(userRegistrationCommand);

        if (!result.IsSuccessful)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("login")]
    [Produces(typeof(Result<UserLoginResponse>))]
    public async Task<IActionResult> LoginUser([FromBody] UserLoginCommand userLoginCommand)
    {
        var result = await _authenticationService.LoginUserAsync(userLoginCommand);

        if (!result.IsSuccessful)
            return BadRequest(result);

        return Ok(result);
    }
}