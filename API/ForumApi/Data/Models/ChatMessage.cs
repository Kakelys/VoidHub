namespace ForumApi.Data.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int ChatMemberId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletetAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public virtual ChatMember Member { get; set; } = null!;
        public virtual Chat Chat { get; set; } = null!;
    }
}