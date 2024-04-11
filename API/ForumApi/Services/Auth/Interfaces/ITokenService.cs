using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ForumApi.Data.Models;
using ForumApi.DTO.Auth;

namespace ForumApi.Services.Auth.Interfaces
{
    public interface ITokenService
    {
        string Create(List<Claim> claims, DateTime expiresAt, string secret);
        bool Validate(string token, string secret);
        JwtSecurityToken Decode(string token);
        JwtPair CreatePair(Account account);
        Task Revoke(string refreshToken);
    }
}