using ForumApi.DTO.DSearch.Sort;

namespace ForumApi.DTO.DSearch
{
    public class SearchParams
    {
        public SearchSortTypes Sort { get; set; } = SearchSortTypes.New;
        public bool WithPostContent { get; set; } = false;
        public bool PartialTitle { get; set; } = false;
        public bool OnlyDeleted { get; set; } = false;
        public int ForumId { get; set; }
    }
}