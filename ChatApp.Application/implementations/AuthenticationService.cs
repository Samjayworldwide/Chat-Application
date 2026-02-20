using System.Diagnostics.CodeAnalysis;
using ChatApp.Application.commands;
using ChatApp.Application.commons;
using ChatApp.Application.interfaces;
using ChatApp.Application.validators;
using ChatApp.Domain.entities;
using ChatApp.Infrastructure.repositories.interfaces;
using ChatApp.SharedKernel.dtos.response;
using Microsoft.Extensions.Logging;

namespace ChatApp.Application.implementations;

[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;

    private readonly IEmailVerificationService _emailVerificationService;

    private readonly ITokenGeneratorService _tokenGeneratorService;

    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(IUserRepository userRepository, IEmailVerificationService emailVerificationService,
        ITokenGeneratorService tokenGeneratorService, ILogger<AuthenticationService> logger)
    {
        _userRepository = userRepository;

        _emailVerificationService = emailVerificationService;

        _tokenGeneratorService = tokenGeneratorService;

        _logger = logger;
    }

    public async Task<Result<string>> RegisterUserAsync(UserRegistrationCommand userRegistrationCommand)
    {
        var validationResult = await new UserRegistrationCommandValidator().ValidateAsync(userRegistrationCommand);

        if (!validationResult.IsValid)
        {
            var validationErrors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

            return Result<string>.ValidationFailure("Validation error", validationErrors);
        }

        var existsByEmailOrUsername = await _userRepository
            .ExistsByEmailOrUsernameAsync(userRegistrationCommand.Email!, userRegistrationCommand.Username!);

        if (existsByEmailOrUsername)
            return Result<string>.Failure("User with given email or username already exists");

        var isEmailVerified = await _emailVerificationService.IsEmailVerifiedAsync(userRegistrationCommand.Email!);

        if (!isEmailVerified)
            return Result<string>.Failure("Email is not verified. Please verify your email before registering.");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegistrationCommand.Password);

        var user = User.Create(userRegistrationCommand.Firstname!, userRegistrationCommand.Lastname!,
            userRegistrationCommand.Username!, userRegistrationCommand.Email!, hashedPassword);

        await _userRepository.CreateUserAsync(user);

        _logger.LogInformation("User with email {Email} and username {Username} registered successfully",
            userRegistrationCommand.Email, userRegistrationCommand.Username);

        return Result<string>.Success("User registered successfully");
    }

    public async Task<Result<UserLoginResponse>> LoginUserAsync(UserLoginCommand userLoginCommand)
    {
        var validationResult = await new UserLoginCommandValidator().ValidateAsync(userLoginCommand);

        if (!validationResult.IsValid)
        {
            var validationErrors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

            return Result<UserLoginResponse>.ValidationFailure("Validation error", validationErrors);
        }

        var user = await _userRepository.GetByEmailOrUsernameAsync(userLoginCommand.EmailOrUsername!);

        if (user == null)
            return Result<UserLoginResponse>.Failure("Invalid email/username or password");

        var passwordMatches = BCrypt.Net.BCrypt.Verify(userLoginCommand.Password, user.Password);

        if (!passwordMatches)
            return Result<UserLoginResponse>.Failure("Invalid email/username or password");

        var jwtToken = _tokenGeneratorService.GenerateJwtToken(user);

        var response = new UserLoginResponse
        {
            Id = user.Id!,
            JwtToken = jwtToken,
            Username = user.Username!
        };

        _logger.LogInformation("User with email/username {EmailOrUsername} logged in successfully",
            userLoginCommand.EmailOrUsername);

        return Result<UserLoginResponse>.Success(response, "Logged in successfully");
    }
}