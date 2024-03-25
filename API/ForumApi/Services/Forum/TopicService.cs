using AutoMapper;
using ForumApi.Data.Models;
using ForumApi.Data.Repository.Extensions;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.Auth;
using ForumApi.DTO.DPost;
using ForumApi.DTO.DTopic;
using ForumApi.DTO.Page;
using ForumApi.Utils.Exceptions;
using ForumApi.Services.ForumS.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Services.ForumS
{
    public class TopicService : ITopicService
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _rep;

        public TopicService(IMapper mapper, IRepositoryManager rep)
        {
            _mapper = mapper;
            _rep = rep;
        }

        public async Task<TopicResponse?> GetTopic(int id, bool allowDeleted = false)
        {
            var firstPost = await _rep.Post.Value
                .FindByCondition(p => p.TopicId == id)
                .AllowDeletedWithTopic(allowDeleted)
                .Include(p => p.Author)
                .OrderBy(p => p.CreatedAt)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Topic not found");

            return await _rep.Topic.Value
                .FindByCondition(t => t.Id == id)
                .AllowDeleted(allowDeleted)
                .Select(t => new TopicResponse(t)
                {
                    Post = new PostResponse(firstPost)
                    {
                        Author = _mapper.Map<User>(firstPost.Author)
                    },
                    CommentsCount = firstPost.CommentsCount
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<TopicResponse>> GetTopics(Offset offset, DateTime time)
        {
            return await _rep.Topic.Value
                .FindByCondition(t => t.DeletedAt == null && t.CreatedAt < time.ToUniversalTime())
                .OrderByDescending(t => t.CreatedAt)
                .TakeOffset(offset)
                .Select(t => new {
                    Topic = t,
                    FirstPost = t.Posts
                        .Where(p => p.AncestorId == null)
                        .OrderBy(p => p.CreatedAt)
                        .First()
                })
                .Select(t => new TopicResponse(t.Topic)
                {
                    Post = new PostResponse(t.FirstPost)
                    {
                        Author = _mapper.Map<User>(t.FirstPost.Author)
                    },
                    CommentsCount = t.FirstPost.CommentsCount
                })
                .ToListAsync();
        }

        public async Task<List<TopicElement>> GetTopics(int forumId, Page page)
        {
            return await _rep.Topic.Value
                .FindByCondition(t => t.ForumId == forumId)
                .AllowDeleted()
                .OrderByDescending(t => t.IsPinned)
                .ThenByDescending(t => t.CreatedAt)
                .Select(t => new TopicElement(t)
                {
                    Author = _mapper.Map<User>(t.Author),
                    LastPost = t.Posts
                        .Where(p => p.DeletedAt == null)
                        .OrderByDescending(p => p.CreatedAt)
                        .Select(p => new LastPost
                        {
                            Id = p.Id,
                            CreatedAt = p.CreatedAt,
                            Author = _mapper.Map<User>(p.Author)
                        }).FirstOrDefault()
                })
                .TakePage(page)
                .ToListAsync();
        }

        public async Task<Topic> Create(int authorId, TopicNew topicDto)
        {
            var forum = await _rep.Forum.Value
                .FindByCondition(f => f.Id == topicDto.ForumId)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Forum not found");

            if(forum.IsClosed)
                throw new BadRequestException("Forum is closed");

            var topic = _mapper.Map<Topic>(topicDto);
            topic.AccountId = authorId;

            var post = new Post
            {
                Content = topicDto.Content, 
                AccountId = authorId
            };

            topic.Posts.Add(post);

            _rep.Topic.Value.Create(topic);
            await _rep.Save();
            return topic;
        }

        public async Task<Topic> Update(int topicId, TopicDto topicDto)
        {
            var entity = await _rep.Topic.Value
                .FindByCondition(t => t.Id == topicId && t.DeletedAt == null, true)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Topic not found");

            _mapper.Map(topicDto, entity);
            await _rep.Save();

            return entity;
        }

        public async Task Delete(int topicId)
        {
            var topicEntity = await _rep.Topic.Value
                .FindByCondition(t => t.Id == topicId && t.DeletedAt == null, true)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Topic not found");

            _rep.Topic.Value.Delete(topicEntity);

            // also delete posts
            // use same time as in topic, so it can be easy recover after
            _rep.Post.Value.DeleteMany(
                _rep.Post.Value
                    .FindByCondition(p => p.TopicId == topicEntity.Id && p.DeletedAt == null, true)
                    .ToList(), 
                topicEntity.DeletedAt
            );

            await _rep.Save();
        }
    }
}