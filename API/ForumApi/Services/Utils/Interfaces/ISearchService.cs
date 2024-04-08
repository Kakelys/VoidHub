using ForumApi.DTO.DSearch;
using ForumApi.DTO.Utils;

namespace ForumApi.Services.Utils.Interfaces
{
    public interface ISearchService
    {
        Task<SearchResponse> SearchTopics(string query, SearchParams search, Page page);
    }
}