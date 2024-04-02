namespace ForumApi.DTO.DChat
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int ChatMemberId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletetAt { get; set; }
        public DateTime? ModifiedAt { get; set; }        
    }
}