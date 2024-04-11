using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;

namespace ForumApi.Data.Repository.Implements
{
    public class SectionRepository(ForumDbContext context) : RepositoryBase<Section>(context), ISectionRepository
    {
    }
}