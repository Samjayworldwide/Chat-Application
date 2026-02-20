using System.Diagnostics.CodeAnalysis;
using ChatApp.Application.commons;
using ChatApp.Application.interfaces;
using ChatApp.SharedKernel.dtos.request;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.WebApi.Controllers;

[ApiController]
[Route("api/email-verification")]
[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
public class EmailVerificationCodeController : ControllerBase
{
    private readonly IEmailVerificationService _emailVerificationService;

    public EmailVerificationCodeController(IEmailVerificationService emailVerificationService)
    {
        _emailVerificationService = emailVerificationService;
    }
    
    [HttpPost("send-code/{email:required}")]
    [Produces(typeof(Result<string>))]
    public async Task<IActionResult> SendVerificationCode(string email)
    {
        var result = await _emailVerificationService.SendVerificationCodeToEmailAsync(email);

        if (!result.IsSuccessful)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpPost("verify-code")]
    [Produces(typeof(Result<string>))]
    public async Task<IActionResult> VerifyCode([FromBody] EmailVerificationRequest emailVerificationRequest)
    {
        var result = await _emailVerificationService.VerifyCodeAsync(emailVerificationRequest);
        
        if (!result.IsSuccessful)
            return BadRequest(result);
        
        return Ok(result);
    }
}