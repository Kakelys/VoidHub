using ForumApi.Data.Repository.Interfaces;

namespace ForumApi.Data.Repository.Implements
{
    public class FileRepository(ForumDbContext context) : RepositoryBase<Models.File>(context), IFileRepository
    {
    }
}