using ForumApi.DTO.Auth;

namespace ForumApi.DTO.DTopic;

public class TopicLast
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User User { get; set; }
}