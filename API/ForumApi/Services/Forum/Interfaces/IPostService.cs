using ForumApi.Data.Models;
using ForumApi.DTO.DPost;
using ForumApi.DTO.Utils;

namespace ForumApi.Services.ForumS.Interfaces;

public interface IPostService
{
    Task<List<PostResponse>> GetPostComments(int? ancestorId, Offset page, Params prms);
    Task<List<PostInfoResponse>> GetPosts(Offset offset, Params prms);
    /// <summary>
    /// Run in transaction
    /// </summary>
    Task<Post> Create(int accountId, PostEditDto postDto);
    Task<Post> Update(int postId, PostEditDto postDto);
    Task Delete(int postId);
}