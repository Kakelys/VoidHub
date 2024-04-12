using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;
using ForumApi.Utils.Extensions;

namespace ForumApi.DTO.DAccount
{
    public class AccountDtoValidator : AbstractValidator<AccountDto>
    {
        /// <summary>
        /// for user to change their own account
        /// </summary>
        public AccountDtoValidator(IJsonStringLocalizer locale)
        {
            // restrict user from changing their own role
            RuleFor(x => x.Role)
                .Must(x => x == Data.Models.RoleEnum.None)
                .WithMessage(locale["validators.no-role"]);

            RuleFor(r => r.Email)
                .EmailRules(locale);

            RuleFor(r => r.Username)
                .UsernameRules(locale);

            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .WithMessage(locale["validators.old-password-required"])
                .When(x => !string.IsNullOrEmpty(x.NewPassword));

            RuleFor(x => x.NewPassword).NotEmpty()
                .WithMessage(locale["validators.new-password-required"])
                .When(x => !string.IsNullOrEmpty(x.OldPassword));

            RuleFor(r => r.NewPassword)
                .PasswordRules(locale)
                .When(x => !string.IsNullOrEmpty(x.NewPassword));
        }
    }
}