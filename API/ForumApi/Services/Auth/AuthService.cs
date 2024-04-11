using AutoMapper;
using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.Auth;
using ForumApi.Utils.Exceptions;
using ForumApi.Utils.Extensions;
using ForumApi.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ForumApi.Services.Auth.Interfaces;
using AspNetCore.Localizer.Json.Localizer;

namespace ForumApi.Services.Auth
{
    public class AuthService(
        IRepositoryManager rep,
        ITokenService tokenService,
        IOptions<JwtOptions> jwtOptions,
        IMapper mapper,
        IJsonStringLocalizer locale) : IAuthService
    {
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;

        public async Task<AuthResponse> RefreshPair(string refreshToken)
        {
            var tokenEntity = await rep.Token.Value.FindByTokenWithAccount(refreshToken, true)
                .FirstOrDefaultAsync() ??  throw new NotFoundException(locale["errors.no-token"]);

            if(tokenEntity.Account.DeletedAt != null)
                throw new BadRequestException(locale["errors.no-refresh-for-deleted-account"]);

            if(tokenEntity.ExpiresAt < DateTime.UtcNow)
                throw new BadRequestException(locale["errors.expired-token"]);

            var pair = tokenService.CreatePair(tokenEntity.Account);

            // fix accidental multiple update
            // what a meme :D
            if (tokenEntity.ExpiresAt.AddDays(1) < DateTime.UtcNow)
            {
                tokenEntity.RefreshToken = pair.RefreshToken;
                tokenEntity.ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.RefreshLifetimeInMinutes);
            }
            else
            {
                pair.RefreshToken = tokenEntity.RefreshToken;
            }

            //update last logged date
            tokenEntity.Account.LastLoggedAt = DateTime.UtcNow;

            await rep.Save();

            return new AuthResponse
            {
                Tokens = pair,
                User = mapper.Map<AuthUser>(tokenEntity.Account)
            };
        }

        public async Task<AuthResponse> Login(Login auth)
        {
            var account = await rep.Account.Value
                .FindByCondition(a => a.LoginName == auth.LoginName && a.DeletedAt == null, true)
                .FirstOrDefaultAsync() ?? throw new BadRequestException(locale["errors.invalid-login-or-password"]);

            if(!PasswordHelper.Verify(auth.Password, account.PasswordHash))
                throw new BadRequestException(locale["errors.invalid-login-or-password"]);

            //check max tokens count and update
            if(account.Tokens.Count > _jwtOptions.MaxTokenCount)
            {
                var token = account.Tokens.OrderBy(t => t.ExpiresAt).First();
                rep.Token.Value.Delete(token);
            }

            var newPair = tokenService.CreatePair(account);

            account.Tokens.Add(new Token
            {
                AccountId = account.Id,
                RefreshToken = newPair.RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.RefreshLifetimeInMinutes)
            });

            account.LastLoggedAt = DateTime.UtcNow;

            await rep.Save();

            return new AuthResponse
            {
                Tokens = newPair,
                User = mapper.Map<AuthUser>(account)
            };
        }

        public async Task<AuthResponse> Register(Register auth)
        {
            if(await rep.Account.Value.FindByLogin(auth.LoginName).AnyAsync())
                throw new BadRequestException(locale["errors.same-login"]);

            if(await rep.Account.Value.FindByUsername(auth.Username).AnyAsync())
                throw new BadRequestException(locale["errors.same-username"]);

            if(await rep.Account.Value.FindByEmail(auth.Email).AnyAsync())
                throw new BadRequestException(locale["errors.same-email"]);

            var account = new Account()
            {
                Username = auth.Username,
                LoginName = auth.LoginName,
                Email = auth.Email,
                PasswordHash = PasswordHelper.Hash(auth.Password),
                Role = Role.User
            };

            //transaction needed for token generation, because neeeds account id
            await rep.BeginTransaction();
            try 
            {
                rep.Account.Value.Create(account);
                await rep.Save();

                var pair = tokenService.CreatePair(account);

                account.Tokens.Add(new Token
                {
                    AccountId = account.Id,
                    RefreshToken = pair.RefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.RefreshLifetimeInMinutes)
                });

                await rep.Save();
                await rep.Commit();

                return new AuthResponse
                {
                    Tokens = pair,
                    User = mapper.Map<AuthUser>(account)
                };
            } 
            catch 
            {
                await rep.Rollback();
                throw; 
            }
        }
    }
}