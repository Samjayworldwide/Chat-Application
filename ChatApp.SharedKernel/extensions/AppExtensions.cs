namespace ChatApp.SharedKernel.extensions;

public static class AppExtensions
{
    private static readonly Random Random = new();

    public const string UserDocumentName = "Users";
    
    public const string PrivateMessageDocumentName = "PrivateMessages";

    public const string EmailVerificationDocumentName = "EmailVerifications";
    
    public const string GroupDocumentName = "Groups";
    
    public const string GroupMessageDocumentName = "GroupMessages";
    
    public const string ChatDocumentName = "Chats";

    public const string EmailVerificationSubject = "Your Email Verification Code";
    
    // Server → Client
    public const string ReceivePrivateMessage = nameof(ReceivePrivateMessage);
    
    public const string ReceiveGroupMessage = nameof(ReceiveGroupMessage);
    
    public const string MessageRead = nameof(MessageRead);
    
    public const string GroupMessagesRead = nameof(GroupMessagesRead);
    
    public const string MessageEdited = nameof(MessageEdited);
    
    public const string ExitGroup = nameof(ExitGroup);

    // Client → Server
    public const string SendPrivateMessage = nameof(SendPrivateMessage);
    
    public const string SendGroupMessage = nameof(SendGroupMessage);
    
    public const string MarkMessageAsRead = nameof(MarkMessageAsRead);

    public static string GenerateVerificationCode()
    {
        var code = Random.Next(100000, 1000000);

        return code.ToString();
    }

    public static DateTime GetLocalDateTime()
    {
        return DateTime.UtcNow.AddHours(1);
    }

    public static string EmailVerificationMailBody(string verificationCode)
    {
        return $"""
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset="UTF-8">
                    <title>Verify Your Email</title>
                </head>
                <body style="font-family: Arial, sans-serif; padding: 20px; background-color: #f5f5f5;">
                    <div style="max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 5px;">
                        <h2 style="color: #2196F3;">Verify Your Email Address</h2>
                        
                        <p>Thank you for registering with us! To complete your registration, please verify your email address.</p>
                        
                        <p>Use the verification code below to activate your account:</p>
                        
                        <div style="background-color: #f0f8ff; padding: 20px; border-radius: 4px; margin: 20px 0; text-align: center;">
                            <h1 style="color: #2196F3; margin: 0; font-size: 32px; letter-spacing: 5px;">{verificationCode}</h1>
                        </div>
                        
                        <p>Enter this code on the verification page to activate your account.</p>
                        
                        <p style="background-color: #fff3cd; padding: 15px; border-left: 4px solid #ff9800; border-radius: 4px; margin: 20px 0;">
                            <strong>Important:</strong> This verification code will expire in <strong>15 minutes</strong>.
                        </p>
                        
                        <p>If you did not request this verification code, please ignore this email or contact our support team.</p>
                        
                        <p style="margin-top: 30px;">Best regards,<br>The Team</p>
                        
                        <hr style="margin-top: 40px; border: none; border-top: 1px solid #ddd;">
                    </div>
                </body>
                </html>
                """;
    }
}