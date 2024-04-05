namespace ForumApi.Data.Models
{
    public class Like
    {
        public int AccountId { get; set; }
        public int PostId { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Post Post { get; set; } = null!;
    }
}