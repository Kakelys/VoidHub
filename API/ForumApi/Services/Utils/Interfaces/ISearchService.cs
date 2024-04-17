using ForumApi.DTO.Auth;
using ForumApi.DTO.DSearch;
using ForumApi.DTO.DTopic;
using ForumApi.DTO.Utils;

namespace ForumApi.Services.Utils.Interfaces
{
    public interface ISearchService
    {
        Task<SearchResponse<TopicInfoResponse>> SearchTopics(string query, SearchParams search, Page page);
        Task<SearchResponse<User>> SearchUsers(string query, SearchParams search, Page page);
    }
}