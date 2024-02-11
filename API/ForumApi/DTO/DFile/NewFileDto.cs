namespace ForumApi.DTO.DFile
{
    public class NewFileDto
    {
        public IFormFile File { get; set; }
        public int? PostId { get; set; }
    }
}