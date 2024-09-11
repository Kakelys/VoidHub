using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;
using ForumApi.Utils.Extensions;

namespace ForumApi.DTO.DAccount;

/// <summary>
/// for admin to change user's username
/// </summary>
public class AccountDtoAdminUsernameValidator : AbstractValidator<AccountDto>
{
    public AccountDtoAdminUsernameValidator(IJsonStringLocalizer locale)
    {
        RuleFor(x => x.Username)
            .UsernameRules(locale);

        // admin change only user's usrname
        RuleFor(x => x.Role)
            .Must(x => x == Data.Models.RoleEnum.None)
            .WithMessage(locale["validators.no-role"]);
        RuleFor(r => r.Email).Null();
        RuleFor(r => r.OldPassword).Null();
        RuleFor(r => r.NewPassword).Null();
        RuleFor(r => r.Img).Null();
    }
}