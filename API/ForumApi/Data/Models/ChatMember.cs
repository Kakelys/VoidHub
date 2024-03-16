namespace ForumApi.Data.Models
{
    public class ChatMember
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int ChatId { get; set; }
        public bool IsAdmin { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Chat Chat { get; set; } = null!;

        public virtual List<ChatMessage> Messages { get; set; } = new();
    }
}