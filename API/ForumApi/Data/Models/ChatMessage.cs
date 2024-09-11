using System.Text.Json.Serialization;

namespace ForumApi.Data.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public int ChatMemberId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletetAt { get; set; }
    public DateTime? ModifiedAt { get; set; }

    [JsonIgnore]
    public virtual ChatMember Member { get; set; }
    [JsonIgnore]
    public virtual Chat Chat { get; set; }
}