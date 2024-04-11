using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Data.Repository.Implements
{
    public class PostRepository(ForumDbContext context) : RepositoryBase<Post>(context), IPostRepository
    {
        public new int Delete(Post entity)
        {
            return Delete(entity, DateTime.UtcNow);
        }

        public new int DeleteMany(IEnumerable<Post> entities)
        {
            return DeleteMany(entities, DateTime.UtcNow);
        }

        public int Delete(Post entity, DateTime? deleteTime)
        {
            var deleted = 1;
            entity.DeletedAt = deleteTime;
            deleted += DeleteMany(entity.Comments.Where(c => c.DeletedAt == null));
            return deleted;
        }

        public int DeleteMany(IEnumerable<Post> entities, DateTime? deleteTime)
        {
            var deleted = 0;
            foreach (var entity in entities)
            {
                deleted += Delete(entity, deleteTime);
            }
            return deleted;
        }

        public async Task IncreaseAllAncestorsCommentsCount(int? ancestorId, int value)
        {
            while(ancestorId != null)
            {
                var ancestor = await _context.Posts.FirstAsync(p => p.Id == ancestorId);
                ancestor.CommentsCount += value;
                ancestorId = ancestor.AncestorId;
            }
        }
    }
}