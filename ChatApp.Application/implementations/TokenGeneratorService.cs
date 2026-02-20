using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatApp.Application.interfaces;
using ChatApp.Domain.entities;
using ChatApp.Infrastructure.configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ChatApp.Application.implementations;

[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
public class TokenGeneratorService : ITokenGeneratorService
{
    private readonly JwtSettings _jwtSettings;

    public TokenGeneratorService(IOptions<JwtSettings> options)
    {
        _jwtSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.NameIdentifier, user.Id!),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(issuer: _jwtSettings.Issuer, audience: _jwtSettings.Audience,
            claims: claims, expires: DateTime.UtcNow.AddMinutes(30), signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}