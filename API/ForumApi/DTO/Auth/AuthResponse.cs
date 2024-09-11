namespace ForumApi.DTO.Auth;

public class AuthResponse
{
    public JwtPair Tokens { get; set; }
    public AuthUser User { get; set; }
}