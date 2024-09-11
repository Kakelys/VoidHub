using AspNetCore.Localizer.Json.Localizer;
using FluentValidation;

namespace ForumApi.DTO.DChat;

public class MessageValidator : AbstractValidator<Message>
{
    public MessageValidator(IJsonStringLocalizer locale)
    {
        RuleFor(x => x.Content)
        .NotEmpty().WithMessage(locale["validators.message-required"])
        .MaximumLength(3000).WithMessage(locale["validators.message-length"]);
    }
}