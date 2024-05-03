using ForumApi.Data.Models;
using ForumApi.DTO.DForum;
using ForumApi.DTO.Utils;

namespace ForumApi.Services.ForumS.Interfaces
{
    public interface IForumService
    {
        Task<ForumResponse?> Get(int forumId, Params prms);
        Task<Forum> Create(ForumEdit forumDto);
        Task<Forum> Update(int forumId, ForumEdit forumDto, string newImagePath);
        Task Delete(int forumId);
    }
}