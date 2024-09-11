namespace ForumApi.DTO.Filter;

public class Filter
{
    public int Offset { get; set; }
    public int Take { get; set; }
    public List<Criteria> Criteria { get; set; }
}
