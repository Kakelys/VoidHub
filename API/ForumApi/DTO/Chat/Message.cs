namespace ForumApi.DTO.DChat
{
    public class Message
    {
        public string Content { get; set; } = null!;
        public int TargetId { get; set; }
    }
}