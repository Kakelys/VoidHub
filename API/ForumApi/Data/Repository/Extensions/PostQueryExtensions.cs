using ForumApi.Data.Models;

namespace ForumApi.Data.Repository.Extensions
{
    public static class PostQueryExtensions
    {
        /// <summary>
        /// allow posts in deleted topic (because post deletion set same DeleteAt)
        /// </summary>
        public static IQueryable<Post> AllowDeletedWithTopic(this IQueryable<Post> query, bool allowDeleted = false) =>
            allowDeleted ?
                query.Where(p => p.DeletedAt == p.Topic.DeletedAt || p.DeletedAt == null) :
                query.Where(p => p.DeletedAt == null);

        public static IEnumerable<Post> AllowDeletedWithTopic(this IEnumerable<Post> query, Topic topic, bool allowDeleted = false) =>
            allowDeleted ?
                query.Where(p => p.DeletedAt == topic.DeletedAt || p.DeletedAt == null) :
                query.Where(p => p.DeletedAt == null);
    }
}