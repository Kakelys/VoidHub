namespace ForumApi.DTO.DSearch;

public class SearchResponse<T>
{
    public int SearchCount { get; set; }
    public List<T> Data { get; set; } = [];
}