using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;

namespace ForumApi.Data.Repository.Implements
{
    public class BanRepository(ForumDbContext context) : RepositoryBase<Ban>(context), IBanRepository
    {
        public override void Delete(Ban entity)
        {
            entity.IsActive = false;
        }

        public override void DeleteMany(IEnumerable<Ban> entities)
        {
            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }
    }
}