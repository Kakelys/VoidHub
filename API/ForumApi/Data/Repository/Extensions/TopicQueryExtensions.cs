using ForumApi.Data.Models;

namespace ForumApi.Data.Repository.Extensions
{
    public static class TopicQueryExtensions
    {
        public static IQueryable<Topic> AllowDeleted(this IQueryable<Topic> query, bool allowDeleted = false) =>
            allowDeleted ?
                query :
                query.Where(t => t.DeletedAt == null);
    }
}