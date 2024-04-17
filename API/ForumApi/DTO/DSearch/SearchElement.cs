namespace ForumApi.DTO.DSearch
{
    public class SearchElement
    {
        public SearchElementType Type { get; set; }
        public object Data { get; set; } = null!;
    }
}
