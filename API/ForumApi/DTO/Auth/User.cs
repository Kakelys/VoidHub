namespace ForumApi.DTO.Auth;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public string AvatarPath { get; set; }
    public DateTime CreatedAt { get; set; }
}