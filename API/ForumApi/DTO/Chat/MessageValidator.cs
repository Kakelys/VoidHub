using FluentValidation;

namespace ForumApi.DTO.DChat
{
    public class MessageValidator : AbstractValidator<Message>
    {
        public MessageValidator()
        {
            RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message required")
            .MaximumLength(3000).WithMessage("Too much characters");
        }
    }
}