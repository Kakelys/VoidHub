namespace ForumApi.DTO.Auth;

public class JwtPair
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}