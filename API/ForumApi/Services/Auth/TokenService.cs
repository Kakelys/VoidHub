using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text;
using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.Auth;
using ForumApi.Utils.Exceptions;
using ForumApi.Options;
using ForumApi.Services.Auth.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using AspNetCore.Localizer.Json.Localizer;

namespace ForumApi.Services.Auth;

public class TokenService(
    IRepositoryManager rep,
    IOptions<JwtOptions> jwtOptions,
    IJsonStringLocalizer locale) : ITokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public string Create(List<Claim> claims, DateTime expiresAt, string secret)
    {
        var issuer = _jwtOptions.Issuer;
        var audience = _jwtOptions.Audience;
        var key = Encoding.ASCII.GetBytes(secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials
                (new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return jwtToken;
    }

    public JwtPair CreatePair(Account account)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new(ClaimTypes.Role, account.Role),
        };

        var accessToken = Create(claims, DateTime.UtcNow.AddMinutes(_jwtOptions.AccessLifetimeInMinutes), _jwtOptions.AccessSecret);
        var refreshToken = Create(claims, DateTime.UtcNow.AddMinutes(_jwtOptions.RefreshLifetimeInMinutes), _jwtOptions.RefreshSecret);

        return new JwtPair
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task Revoke(string refreshToken)
    {
        var tokenEntity = await rep.Token.Value.FindByToken(refreshToken, false)
            .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-token"]);

        rep.Token.Value.Delete(tokenEntity);

        await rep.Save();
    }

    public bool Validate(string token, string secret)
    {
        var validator = new JwtSecurityTokenHandler();

        var validationParams = new TokenValidationParameters
        {
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        if (validator.CanReadToken(token))
        {
            try
            {
                validator.ValidateToken(token, validationParams, out var validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        return false;
    }

    public JwtSecurityToken Decode(string token)
    {
        return new JwtSecurityTokenHandler().ReadJwtToken(token);
    }
}