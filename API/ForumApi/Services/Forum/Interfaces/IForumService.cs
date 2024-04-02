using ForumApi.Data.Models;
using ForumApi.DTO.DForum;

namespace ForumApi.Services.ForumS.Interfaces
{
    public interface IForumService
    {
        Task<ForumResponse?> Get(int forumId);
        Task<Forum> Create(ForumDto forumDto);
        Task<Forum> Update(int forumId, ForumDto forumDto);
        Task Delete(int forumId);
    }
}