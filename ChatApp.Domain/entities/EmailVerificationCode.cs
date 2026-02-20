using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Domain.entities;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class EmailVerificationCode
{
    public string? Id { get; set; }
    
    public string? Email { get; set; }
    
    public string? VerificationCode { get; set; }
    
    public bool? IsVerified { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(1);
    
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(1).AddMinutes(15);
}