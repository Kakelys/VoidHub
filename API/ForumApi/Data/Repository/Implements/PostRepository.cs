using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Data.Repository.Implements
{
    public class PostRepository : RepositoryBase<Post>, IPostRepository
    {
        public PostRepository(ForumDbContext context) : base(context)
        {
        }

        public new int Delete(Post entity)
        {
            var deleted = 1;
            entity.DeletedAt = DateTime.UtcNow;
            deleted += DeleteMany(entity.Comments.Where(c => c.DeletedAt == null));
            return deleted;
        }

        public new int DeleteMany(IEnumerable<Post> entities)
        {
            var deleted = 0;
            foreach (var entity in entities)
            {
                deleted += Delete(entity);
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