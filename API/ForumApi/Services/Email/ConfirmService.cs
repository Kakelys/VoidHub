using System.Security.Claims;
using System.Text;
using AspNetCore.Localizer.Json.Localizer;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.Options;
using ForumApi.Services.Auth.Interfaces;
using ForumApi.Services.Email.Interfaces;
using ForumApi.Utils.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ForumApi.Services.Email;

public class ConfirmService(
    IEmailService emailService,
    ITokenService tokenService,
    IOptions<JwtOptions> jwtOptions,
    IJsonStringLocalizer locale,
    IRepositoryManager rep,
    IConfiguration config,
    ILogger<ConfirmService> logger) : IConfirmService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task ConfirmEmail(string base64Token)
    {
        byte[] base64Encoded;

        try
        {
            base64Encoded = Convert.FromBase64String(base64Token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Confirm token not parsed");
            throw new BadRequestException(locale["errors.invalid-token"]);
        }

        var token = Encoding.UTF8.GetString(base64Encoded);

        if (!tokenService.Validate(token, _jwtOptions.ConfirmSecret))
            throw new BadRequestException(locale["errors.invalid-token"]);

        var deToken = tokenService.Decode(token);
        var accountId = int.Parse(deToken.Claims.First(c => c.Type == "nameid").Value);

        var user = await rep.Account.Value
            .FindById(accountId, true)
            .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-user"]);

        if (user.IsEmailConfirmed)
            return;

        user.IsEmailConfirmed = true;
        await rep.Save();
    }

    public async Task SendConfirmEmail(int accountId)
    {
        var user = await rep.Account.Value
            .FindById(accountId)
            .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-user"]);

        if (user.IsEmailConfirmed)
            throw new BadRequestException(locale["errors.email-already-confirmed"]);

        List<Claim> claims = [new(ClaimTypes.NameIdentifier, user.Id.ToString())];

        var token = tokenService.Create(claims, DateTime.UtcNow.AddMinutes(_jwtOptions.ConfirmLifetimeInMinutes), _jwtOptions.ConfirmSecret);

        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var tokenBase = Convert.ToBase64String(tokenBytes);

        var msg = $"{locale["email.confirm-ask-for-confirm-line"]}";
        msg += $"\n{locale["email.link-will-be-valid"]} {_jwtOptions.ConfirmLifetimeInMinutes / 60} {locale["email.hours"]}";
        msg += $"\n{locale["email.not-for-smap"]} 😂👺🤡";
        msg += $"\n{config["ClientConfirmLink"]}/{tokenBase}";

        await emailService.Send
        (
            $"VoidHub - {locale["email.confirm-email"]}",
            msg,
            user.Email,
            MimeKit.Text.TextFormat.Plain
        );
    }
}