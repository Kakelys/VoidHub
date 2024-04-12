using System.Security.Claims;
using System.Text;
using AspNetCore.Localizer.Json.Localizer;
using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.Options;
using ForumApi.Services.Auth.Interfaces;
using ForumApi.Services.Email.Interfaces;
using ForumApi.Utils.Exceptions;
using ForumApi.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit.Text;

namespace ForumApi.Services.Email
{
    public class PasswordRecoverService(
        IEmailService emailService,
        IRepositoryManager rep,
        IJsonStringLocalizer locale,
        IOptions<JwtOptions> jwtOptions,
        ITokenService tokenService,
        IConfiguration config
    ) : IPasswordRecoverService
    {
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;

        public async Task Recover(string base64Token, string password)
        {
            var base64Encoded = Convert.FromBase64String(base64Token);
            var token = Encoding.UTF8.GetString(base64Encoded);

            if(!tokenService.Validate(token, _jwtOptions.RecoverSecret))
                throw new BadRequestException(locale["errors.invalid-token"]);

            var deToken =  tokenService.Decode(token);
            var accountId = int.Parse(deToken.Claims.First(c => c.Type == "nameid").Value);

            var user = await rep.Account.Value
                .FindById(accountId, true)
                .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-user"]);

            user.PasswordHash = PasswordHelper.Hash(password);
            await rep.Save();
        }

        public async Task SendRecoverEmail(Account user)
        {
            List<Claim> claims = [new (ClaimTypes.NameIdentifier, user.Id.ToString())];

            var token = tokenService.Create(claims, DateTime.UtcNow.AddMinutes(_jwtOptions.RecoverLifetimeInMinutes), _jwtOptions.RecoverSecret);

            var tokenBytes = Encoding.UTF8.GetBytes(token);
            var tokenBase = Convert.ToBase64String(tokenBytes);

            var isHours = _jwtOptions.RecoverLifetimeInMinutes > 60;
            var timeValue = isHours ? _jwtOptions.RecoverLifetimeInMinutes / 60 : _jwtOptions.RecoverLifetimeInMinutes;

            var msg = $"{locale["email.password-recover-request"]}";
            msg += $"\n{locale["email.ignore-if-not-you"]}";
            msg += $"\n\n{locale["email.link-will-be-valid"]} {timeValue} {(isHours ? locale["email.hours"] : locale["email.mins"])}";
            msg += $"\n{locale["email.visit-link-enter-password"]}";
            msg += $"\n\n{config["ClientRecoverLink"]}/{tokenBase}";

            await emailService.Send(
                $"VoidHub - {locale["email.password-recover"]}",
                msg,
                user.Email,
                TextFormat.Text
            );
        }
    }
}