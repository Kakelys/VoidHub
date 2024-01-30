using AutoMapper;
using ForumApi.Data.Models;
using ForumApi.Data.Repository.Extensions;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.Auth;
using ForumApi.DTO.DPost;
using ForumApi.DTO.DTopic;
using ForumApi.DTO.Page;
using ForumApi.Utils.Exceptions;
using ForumApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Services
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

        public async Task<TopicResponse?> GetTopic(int id)
        {
            var firstPost = await _rep.Post.Value
                .FindByCondition(p => p.TopicId == id && p.DeletedAt == null && p.AncestorId == null)
                .Include(p => p.Author)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Topic not found");

            return await _rep.Topic.Value
                .FindByCondition(t => t.Id == id && t.DeletedAt == null)
                .Select(p => new TopicResponse
                {
                    Id = p.Id,
                    ForumId = p.ForumId,
                    Title = p.Title,
                    CreatedAt = p.CreatedAt,
                    IsClosed = p.IsClosed,
                    IsPinned = p.IsPinned,
                    Post = new PostResponse
                    {
                        Id = firstPost.Id,
                        Content = firstPost.Content ?? "",
                        CreatedAt = firstPost.CreatedAt,
                        Author = _mapper.Map<User>(firstPost.Author)
                    },
                    PostsCount = p.Posts.Where(pp => pp.DeletedAt == null && pp.AncestorId == firstPost.Id).Count(),
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
                        .FirstOrDefault()
                })
                .Select(t => new TopicResponse
                {
                    Id = t.Topic.Id,
                    ForumId = t.Topic.ForumId,
                    Title = t.Topic.Title,
                    CreatedAt = t.Topic.CreatedAt,
                    IsClosed = t.Topic.IsClosed,
                    IsPinned = t.Topic.IsPinned,
                    Post = new PostResponse
                    {
                        Content = t.FirstPost == null ? "" : t.FirstPost.Content,
                        CreatedAt = t.FirstPost == null ? default : t.FirstPost.CreatedAt,
                        Author = _mapper.Map<User>(t.FirstPost.Author)
                    },
                    PostsCount = t.FirstPost.CommentsCount,
                    CommentsCount = 0
                })
                .ToListAsync();
        }

        public async Task<List<TopicElement>> GetTopics(int forumId, Page page)
        {
            return await _rep.Topic.Value
                .FindByCondition(t => t.DeletedAt == null && t.ForumId == forumId)
                .OrderByDescending(t => t.IsPinned)
                .ThenByDescending(t => t.CreatedAt)
                .Select(p => new TopicElement
                {
                    Id = p.Id,
                    Title = p.Title,
                    CreatedAt = p.CreatedAt,
                    IsClosed = p.IsClosed,
                    IsPinned = p.IsPinned,
                    PostsCount = p.Posts.Where(p => p.DeletedAt == null).Count(),
                    Author = _mapper.Map<User>(p.Author),
                    LastPost = p.Posts
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
            var entity = await _rep.Topic.Value
                .FindByCondition(t => t.Id == topicId && t.DeletedAt == null, true)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Topic not found");

            _rep.Topic.Value.Delete(entity);
            await _rep.Save();
        }
    }
}