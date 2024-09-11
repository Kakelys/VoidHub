using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;

namespace ForumApi.Data.Repository.Implements;

public class LikeRepository(ForumDbContext context) : RepositoryBase<Like>(context), ILikeRepository
{
}