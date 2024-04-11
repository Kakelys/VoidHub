using MimeKit.Text;

namespace ForumApi.Services.Email.Interfaces
{
    public interface IEmailService
    {
        Task Send(string subject, string message, string email, TextFormat format);
    }
}