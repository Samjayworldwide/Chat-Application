using ChatApp.Domain.entities;

namespace ChatApp.Application.interfaces;

public interface ITokenGeneratorService
{
    string GenerateJwtToken(User user);
}