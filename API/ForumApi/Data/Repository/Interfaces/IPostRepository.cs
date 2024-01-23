using ForumApi.Data.Models;

namespace ForumApi.Data.Repository.Interfaces
{
    public interface IPostRepository : IRepositoryBase<Post>
    {
        Task IncreaseAllAncestorsCommentsCount(int? ancestorId, int value);
        
        /// <summary>
        /// Return count inner comments
        /// </summary>
        new int Delete(Post entity);

        /// <summary>
        /// Return count inner comments (same as delete)
        /// </summary>
        new int DeleteMany(IEnumerable<Post> entities);
    }
}