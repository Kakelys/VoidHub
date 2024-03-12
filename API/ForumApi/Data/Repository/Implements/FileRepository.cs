using ForumApi.Data.Repository.Interfaces;

namespace ForumApi.Data.Repository.Implements
{
    public class FileRepository : RepositoryBase<Models.File>, IFileRepository
    {
        public FileRepository(ForumDbContext context) : base(context)
        {
        }
    }
}