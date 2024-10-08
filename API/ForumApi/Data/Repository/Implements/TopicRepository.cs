using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;

namespace ForumApi.Data.Repository.Implements;

public class TopicRepository(ForumDbContext context) : RepositoryBase<Topic>(context), ITopicRepository
{
    public override void Delete(Topic entity)
    {
        entity.DeletedAt = DateTime.UtcNow;
    }

    public override void DeleteMany(IEnumerable<Topic> entities)
    {
        foreach (var entity in entities)
        {
            Delete(entity);
        }
    }
}