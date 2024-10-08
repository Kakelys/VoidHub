using System.Text.Json.Serialization;

namespace ForumApi.Data.Models;

public class Forum
{
    public int Id { get; set; }
    public int SectionId { get; set; }
    public string Title { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsClosed { get; set; }
    public string ImagePath { get; set; }

    [JsonIgnore]
    public virtual Section Section { get; set; }
    [JsonIgnore]
    public virtual List<Topic> Topics { get; set; } = [];
}