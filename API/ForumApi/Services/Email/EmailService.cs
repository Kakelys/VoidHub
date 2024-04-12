using ForumApi.Services.Email.Interfaces;
using ForumApi.Utils.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace ForumApi.Services.Email
{
    public class EmailService(IOptions<EmailOptions> emailOptions) : IEmailService
    {
        private readonly EmailOptions _emailOptions = emailOptions.Value;

        public async Task Send(string subject, string message, string email, TextFormat format)
        {
            var from = MailboxAddress.Parse(_emailOptions.From);
            var to = MailboxAddress.Parse(email);
            var msg = new MimeMessage
            {
                Subject = subject,
                Body = new TextPart(format)
                {
                    Text = message
                },
                From = { from },
                To = { to }
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailOptions.Host, _emailOptions.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailOptions.From, _emailOptions.AuthKey);
            await smtp.SendAsync(msg);
            await smtp.DisconnectAsync(true);
        }
    }
}