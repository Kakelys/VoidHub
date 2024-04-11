using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;

namespace ForumApi.Data.Repository.Implements
{
    public class ForumRepository(ForumDbContext context) : RepositoryBase<Forum>(context), IForumRepository
    {
        public override void Delete(Forum entity)
        {
            entity.DeletedAt = DateTime.UtcNow;
        }

        public override void DeleteMany(IEnumerable<Forum> entities)
        {
            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }
    }
}