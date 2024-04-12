using ForumApi.Data.Models;

namespace ForumApi.Services.Email.Interfaces
{
    public interface IPasswordRecoverService
    {
        Task Recover(string base64Token, string password);
        Task SendRecoverEmail(Account user);
    }
}