using ForumApi.DTO.Auth;

namespace ForumApi.DTO.Utils;

public class ConnectedUser
{
    public User User { get; set; } = null!;
    public List<string> ConnectionIds { get; set; }
}