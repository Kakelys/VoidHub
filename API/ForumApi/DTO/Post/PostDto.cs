namespace ForumApi.DTO.DPost;

public class PostDto
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public int TopicId { get; set; }
    public int? AncestorId { get; set; }
    public int CommentsCount { get; set; }
    public int LikesCount { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public bool IsLiked { get; set; }
}