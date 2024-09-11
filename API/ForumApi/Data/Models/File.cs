using System.Text.Json.Serialization;

namespace ForumApi.Data.Models;

public class File
{
    public int Id { get; set; }
    public string Path { get; set; }
    public int AccountId { get; set; }
    public int? PostId { get; set; }
    public DateTime CreatedAt { get; set; }

    [JsonIgnore]
    public virtual Account Account { get; set; }
    [JsonIgnore]
    public virtual Post Post { get; set; }
}