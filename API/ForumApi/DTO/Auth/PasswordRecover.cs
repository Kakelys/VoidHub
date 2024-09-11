namespace ForumApi.DTO.Auth;

public class PasswordRecover
{
    public string Base64Token { get; set; } = null!;
    public string Password { get; set; } = null!;
}