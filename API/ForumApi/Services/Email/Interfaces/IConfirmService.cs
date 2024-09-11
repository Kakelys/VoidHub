namespace ForumApi.Services.Email.Interfaces;

public interface IConfirmService
{
    Task SendConfirmEmail(int accountId);
    Task ConfirmEmail(string base64Token);
}