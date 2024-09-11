using System.Text.Json.Serialization;

namespace ForumApi.Data.Models;

public class Ban
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public int ModeratorId { get; set; }
    public int UpdatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Reason { get; set; }
    public bool IsActive { get; set; } = true;

    [JsonIgnore]
    public virtual Account Account { get; set; }
    [JsonIgnore]
    public virtual Account Moderator { get; set; }
    [JsonIgnore]
    public virtual Account UpdatedBy { get; set; }
}