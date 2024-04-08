namespace ForumApi.DTO.DSearch
{
    public class SearchParams
    {
        public string Sort { get; set; } = "";
        public bool WithPostContent { get; set; } = false;
        public bool OnlyDeleted { get; set; } = false;
    }
}