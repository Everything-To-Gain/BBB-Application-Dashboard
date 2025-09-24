using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Domain.ValueObjects;
using BBB_ApplicationDashboard.Infrastructure.Exceptions.User;
using Microsoft.IdentityModel.Tokens;

namespace BBB_ApplicationDashboard.Infrastructure.Services.Auth;

public class JwtTokenService(ISecretService secretService) : IJwtTokenService
{
    public string GenerateJwtToken(int expirationDays, Domain.Entities.User user)
    {
        //!1) Key
        var key = secretService.GetSecret(ProjectSecrets.AuthSecretKey, Folders.Auth);

        if (key.Length < 64)
            throw new UserBadRequestException("Key length must be at least 64 characters!");

        //!2) Claims
        List<Claim> claims =
        [
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.UserSource.ToString()),
        ];

        //!3) Token descriptor
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Subject = new ClaimsIdentity(claims),
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(expirationDays),
        };

        //!4) Token handler
        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private ClaimsPrincipal ExtractClaimsPrincipal(string token)
    {
        var securityTokenHandler = new JwtSecurityTokenHandler();

        if (!securityTokenHandler.CanReadToken(token))
            throw new UserUnauthorizedException("Token has been malformed or not valid JWT!");
        return securityTokenHandler.ValidateToken(
            token,
            new()
            {
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(
                        secretService.GetSecret(ProjectSecrets.AuthSecretKey, Folders.Auth)
                    )
                ),
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuer = false,
            },
            out _
        );
    }

    public string ExtractEmailFromToken(string token)
    {
        var emailClaim =
            ExtractClaimsPrincipal(token)
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)
            ?? throw new UserNotFoundException("Email not found!");
        return emailClaim.Value;
    }
}
