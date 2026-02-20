using System.Diagnostics.CodeAnalysis;
using ChatApp.Application.commons;
using ChatApp.Application.interfaces;
using ChatApp.Application.validators;
using ChatApp.Domain.entities;
using ChatApp.Infrastructure.configurations;
using ChatApp.Infrastructure.models;
using ChatApp.Infrastructure.repositories.interfaces;
using ChatApp.Infrastructure.ServiceBusInfrastructure.Interface;
using ChatApp.SharedKernel.dtos.request;
using ChatApp.SharedKernel.extensions;
using Microsoft.Extensions.Options;


namespace ChatApp.Application.implementations;

[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
public class EmailVerificationService : IEmailVerificationService
{
    private readonly IEmailVerificationRepository _emailVerificationRepository;

    private readonly IServiceBusPublisher _serviceBusPublisher;

    private readonly ServiceBusSettings _serviceBusSettings;

    public EmailVerificationService(IEmailVerificationRepository emailVerificationRepository,
        IServiceBusPublisher serviceBusPublisher, IOptions<ServiceBusSettings> options)
    {
        _serviceBusSettings = options.Value ?? throw new ArgumentNullException(nameof(options));

        _emailVerificationRepository = emailVerificationRepository;

        _serviceBusPublisher = serviceBusPublisher;
    }

    public async Task<Result<string>> SendVerificationCodeToEmailAsync(string email)
    {
        var emailVerificationCode = await _emailVerificationRepository.GetEmailVerificationAsync(email);

        var verificationCode = AppExtensions.GenerateVerificationCode();

        var emailDetails = new EmailDetails
        {
            Body = AppExtensions.EmailVerificationMailBody(verificationCode),
            RecipientEmail = email,
            Subject = AppExtensions.EmailVerificationSubject
        };

        if (emailVerificationCode == null)
        {
            emailVerificationCode = new EmailVerificationCode
            {
                Email = email,
                VerificationCode = verificationCode,
                IsVerified = false
            };

            await _emailVerificationRepository.CreateVerificationTokenAsync(emailVerificationCode);
        }
        else
        {
            emailVerificationCode.VerificationCode = verificationCode;
            emailVerificationCode.CreatedAt = AppExtensions.GetLocalDateTime();
            emailVerificationCode.ExpiresAt = AppExtensions.GetLocalDateTime().AddMinutes(15);
            emailVerificationCode.IsVerified = false;

            await _emailVerificationRepository.UpdateVerificationTokenAsync(emailVerificationCode);
        }

        var pushed = await _serviceBusPublisher
            .PushToServiceBusQueueAsync(emailDetails, _serviceBusSettings.EmailQueueName);

        return !pushed
            ? Result<string>.Failure("Failed to send verification email. Please try again later.")
            : Result<string>.Success($"A verification code has been sent to your email address {email}");
    }


    public async Task<Result<string>> VerifyCodeAsync(EmailVerificationRequest emailVerificationRequest)
    {
        var validationResult = await new EmailVerificationRequestValidator().ValidateAsync(emailVerificationRequest);

        if (!validationResult.IsValid)
        {
            var validationErrors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

            return Result<string>.ValidationFailure("Validation error", validationErrors);
        }

        var emailVerificationCode = await _emailVerificationRepository
            .GetEmailVerificationAsync(emailVerificationRequest.Email!);

        if (emailVerificationCode == null)
            return Result<string>.Failure("No verification request found for this email.");

        if (emailVerificationCode.IsVerified == true)
            return Result<string>.Failure("This email has already been verified.");

        if (emailVerificationCode.ExpiresAt < AppExtensions.GetLocalDateTime())
            return Result<string>.Failure("The verification code has expired. Please request a new one.");

        if (emailVerificationCode.VerificationCode != emailVerificationRequest.VerificationCode)
            return Result<string>.Failure("The provided verification code is incorrect.");

        emailVerificationCode.IsVerified = true;

        await _emailVerificationRepository.UpdateVerificationTokenAsync(emailVerificationCode);

        return Result<string>.Success("Email verified successfully.");
    }

    public async Task<bool> IsEmailVerifiedAsync(string email)
    {
        var emailVerificationCode = await _emailVerificationRepository.GetEmailVerificationAsync(email);

        if (emailVerificationCode == null)
            return false;

        return emailVerificationCode.IsVerified == true;
    }
}