namespace ForumApi.DTO.Utils
{
    public class Params
    {
        public DateTime? BelowTime { get; set; } = null;
        public bool IncludeDeleted { get; set; } = false;
        public bool OnlyDeleted { get; set; } = false;
        public int ByAccountId { get; set; } = 0;
        public string OrderBy { get; set; } = "CreatedAt";

        public static Params FromUser(Params prms)
        {
            return new Params
            {
                BelowTime = prms.BelowTime,
                ByAccountId = prms.ByAccountId,
                OrderBy = prms.OrderBy
            };
        }
    }
}