namespace ForumApi.DTO.Auth;

public class PasswordRecover
{
    public string Base64Token { get; set; }
    public string Password { get; set; }
}