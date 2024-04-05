using ForumApi.DTO.DPost;

namespace ForumApi.Services.ForumS.Interfaces
{
    public interface ILikeService
    {
        Task Like(int accountId, int postId);
        Task UnLike(int accountId, int postId);
        Task UpdateLikeStatus(int accountId, PostDto post);
        //Task UpdateLikeStatus(int accountId, List<PostDto> posts);
    }
}