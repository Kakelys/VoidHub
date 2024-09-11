using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;

namespace ForumApi.Data.Repository.Implements;

public class ForumRepository(ForumDbContext context) : RepositoryBase<Forum>(context), IForumRepository
{
}