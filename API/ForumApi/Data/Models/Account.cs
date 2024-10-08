using System.Text.Json.Serialization;

namespace ForumApi.Data.Models;

public class Account
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string LoginName { get; set; }
    [JsonIgnore]
    public string Email { get; set; }
    [JsonIgnore]
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoggedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string AvatarPath { get; set; }
    public bool IsEmailConfirmed { get; set; }

    [JsonIgnore]
    public virtual List<Token> Tokens { get; set; } = [];
    [JsonIgnore]
    public virtual List<Post> Posts { get; set; } = [];
    [JsonIgnore]
    public virtual List<Topic> Topics { get; set; } = [];
    [JsonIgnore]
    public virtual List<Ban> RecievedBans { get; set; } = [];
    [JsonIgnore]
    public virtual List<Ban> GivenBans { get; set; } = [];
    [JsonIgnore]
    public virtual List<Ban> UpdatedBans { get; set; } = [];
    [JsonIgnore]
    public virtual List<File> UploadedFiles { get; set; } = [];
    [JsonIgnore]
    public virtual List<ChatMember> ChatMembers { get; set; } = [];
    [JsonIgnore]
    public virtual List<Like> Likes { get; set; } = [];
}