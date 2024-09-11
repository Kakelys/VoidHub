using System.Text.Json.Serialization;
using NpgsqlTypes;

namespace ForumApi.Data.Models;

public class Post
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public int TopicId { get; set; }
    public int? AncestorId { get; set; }
    public int CommentsCount { get; set; }
    public int LikesCount { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    [JsonIgnore]
    public NpgsqlTsVector SearchVector { get; set; }

    [JsonIgnore]
    public virtual Topic Topic { get; set; }
    [JsonIgnore]
    public virtual Account Author { get; set; }

    [JsonIgnore]
    public virtual List<Post> Comments { get; set; } = [];
    [JsonIgnore]
    public virtual Post Ancestor { get; set; }

    [JsonIgnore]
    public virtual List<File> Files { get; set; } = [];
    [JsonIgnore]
    public virtual List<Like> Likes { get; set; } = [];
}