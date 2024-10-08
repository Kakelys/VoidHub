using ForumApi.DTO.DTopic;
using ForumApi.DTO.Utils;

namespace ForumApi.Services.ForumS.Interfaces;

public interface ITopicService
{
    /// <summary>
    /// Get topic info, first post and first comments
    /// </summary>
    Task<TopicResponse> GetTopic(int id, bool allowDeleted = false);
    /// <summary>
    /// Load topics on forum
    /// </summary>
    Task<List<TopicElement>> GetTopics(int forumId, Page page, Params prms);
    /// <summary>
    /// Get topics info and first post
    /// </summary>
    Task<List<TopicInfoResponse>> GetTopics(Offset offset, Params prms);
    Task<TopicInfoResponse> Create(int authorId, TopicNew topicDto);
    Task<TopicDto> Update(int topicId, TopicEdit topicDto);
    Task Delete(int topicId);
    Task<TopicDto> Recover(int topicId);
}