namespace ForumApi.DTO.Auth;

public class AuthUser : User
{
    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
}