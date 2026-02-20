using ChatApp.Application.commands;
using ChatApp.Application.commons;
using ChatApp.SharedKernel.dtos.response;

namespace ChatApp.Application.interfaces;

public interface IAuthenticationService
{
    Task<Result<string>> RegisterUserAsync(UserRegistrationCommand userRegistrationCommand);
    
    Task<Result<UserLoginResponse>> LoginUserAsync(UserLoginCommand userLoginCommand);
}