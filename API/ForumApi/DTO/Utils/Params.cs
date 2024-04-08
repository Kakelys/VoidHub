namespace ForumApi.DTO.Utils
{
    public class Params
    {
        public DateTime? BelowTime { get; set; } = null;
        public bool IncludeDeleted { get; set; } = false;
        public int ByAccountId { get; set; } = 0;
        public string OrderBy { get; set; } = "CreatedAt";
    }
}