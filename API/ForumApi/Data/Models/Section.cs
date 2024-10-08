using System.Text.Json.Serialization;

namespace ForumApi.Data.Models;

public class Section
{
    public int Id { get; set; }
    public int OrderPosition { get; set; }
    public string Title { get; set; }
    public bool IsHidden { get; set; }

    [JsonIgnore]
    public virtual List<Forum> Forums { get; set; } = [];
}