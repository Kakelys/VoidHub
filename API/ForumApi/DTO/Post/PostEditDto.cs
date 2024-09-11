namespace ForumApi.DTO.DPost;

public class PostEditDto
{
    public int TopicId { get; set; }
    public string Content { get; set; }
    public int? AncestorId { get; set; }

    public List<int> FileIds { get; set; } = [];
}