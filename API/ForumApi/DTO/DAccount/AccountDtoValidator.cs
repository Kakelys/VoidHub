using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;

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
                .NotEmpty()
                .WithMessage(locale["validators.email-required"])
                .EmailAddress()
                .WithMessage(locale["validators.email-valid"]);

            RuleFor(r => r.Username)
                .NotEmpty()
                .WithMessage(locale["validators.username-required"])
                .Length(3, 32)
                .WithMessage(locale["forms-errors.username-length"])
                .Matches(@"^[a-zA-Z\d!@#$%^&*()\-_=+{}:,<.>]+$")
                .WithMessage(locale["validators.username-characters"]);

            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .WithMessage(locale["validators.old-password-required"])
                .When(x => !string.IsNullOrEmpty(x.NewPassword));

            RuleFor(x => x.NewPassword).NotEmpty()
                .WithMessage(locale["validators.new-password-required"])
                .When(x => !string.IsNullOrEmpty(x.OldPassword));

            RuleFor(r => r.NewPassword)
                .NotEmpty()
                .WithMessage(locale["validators.password-required"])
                .Length(8, 32)
                .WithMessage(locale["forms-errors.password-length"])
                .Matches(@"^[a-zA-Z\d!@#$%^&*()\-_=+{}:,<.>]+$")
                .WithMessage(locale["validators.password-characters"])
                .When(x => !string.IsNullOrEmpty(x.NewPassword));
        }        
    }
}