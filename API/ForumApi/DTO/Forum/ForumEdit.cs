namespace ForumApi.DTO.DForum;

public class ForumEdit
{
    public string Title { get; set; }
    public int SectionId { get; set; }
    public IFormFile Img { get; set; }
}
