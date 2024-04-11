using System.Text.Json.Serialization;

namespace ForumApi.Data.Models
{
    public class File
    {
        public int Id { get; set; }
        public string Path { get; set; } = null!;
        public int AccountId { get; set; }
        public int? PostId { get; set; }

        [JsonIgnore]
        public virtual Account Account { get; set; } = null!;
        [JsonIgnore]
        public virtual Post Post { get; set; } = null!;
    }
}