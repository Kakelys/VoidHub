using ForumApi.DTO.DTopic;

namespace ForumApi.DTO.DSearch
{
    public class SearchResponse
    {
        public int SearchCount { get; set; }
        public List<TopicInfoResponse> Data { get; set; } = new();
    }
}