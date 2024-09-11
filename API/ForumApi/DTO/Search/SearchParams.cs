using ForumApi.DTO.DSearch.Sort;

namespace ForumApi.DTO.DSearch;

public class SearchParams
{
    public SearchSortTypes Sort { get; set; } = SearchSortTypes.New;
    public bool WithPostContent { get; set; }
    public bool PartialTitle { get; set; }
    public bool OnlyDeleted { get; set; }
    public int ForumId { get; set; }
}