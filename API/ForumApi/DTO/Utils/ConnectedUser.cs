using ForumApi.DTO.Auth;

namespace ForumApi.DTO.Utils;

public class ConnectedUser
{
    public User User { get; set; }
    public List<string> ConnectionIds { get; set; }
}