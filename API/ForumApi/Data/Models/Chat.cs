namespace ForumApi.Data.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public bool IsGroup { get; set; }

        public virtual List<ChatMember> Members { get; set; } = new();
        public virtual List<ChatMessage> Messages { get; set; } = new();
    }
}