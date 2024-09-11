using ForumApi.DTO.DBan;

namespace ForumApi.DTO.DAccount;

public class AccountResponse
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public string AvatarPath { get; set; }
    public DateTime CreatedAt { get; set; }
    public int PostsCount { get; set; }
    public int TopicsCount { get; set; }

    public BanEdit Ban { get; set; }
}