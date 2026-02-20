using System.Diagnostics.CodeAnalysis;

namespace ChatApp.SharedKernel.dtos.request;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class EmailVerificationRequest
{
    public string? Email { get; set; }
    
    public string? VerificationCode { get; set; }
}