using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Infrastructure.configurations;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class EmailSettings
{
    public required string SenderName { get; set; }

    public required string SenderEmailAddress { get; set; }

    public required string EmailPassword { get; set; }

    public required string SmtpServer { get; set; }

    public int Port { get; set; }
}