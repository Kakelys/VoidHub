using System.Text.Json.Serialization;
using NpgsqlTypes;

namespace ForumApi.Data.Models;

public class Topic
{
    public int Id { get; set; }
    public int ForumId { get; set; }
    public int AccountId { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsPinned { get; set; }
    public bool IsClosed { get; set; }
    public int PostsCount { get; set; }

    [JsonIgnore]
    public NpgsqlTsVector SearchVector { get; set; }

    [JsonIgnore]
    public virtual Forum Forum { get; set; }
    [JsonIgnore]
    public virtual List<Post> Posts { get; set; } = [];

    [JsonIgnore]
    public virtual Account Author { get; set; }
}