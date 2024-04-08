using System.Threading;
using AutoMapper;
using ForumApi.Data.Models;
using ForumApi.Data.Repository.Extensions;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.Auth;
using ForumApi.DTO.DPost;
using ForumApi.DTO.DTopic;
using ForumApi.DTO.Utils;
using ForumApi.Utils.Exceptions;
using ForumApi.Services.ForumS.Interfaces;
using Microsoft.EntityFrameworkCore;
using LinqKit;

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
                .Select(t => new TopicResponse
                {
                    Topic = _mapper.Map<TopicDto>(t),
                    Post = _mapper.Map<PostDto>(firstPost),
                    Sender = _mapper.Map<User>(firstPost.Author),
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<TopicInfoResponse>> GetTopics(Offset offset, Params prms)
        {
            var predicate = PredicateBuilder.New<Topic>(t => true);
            
            if(prms.BelowTime != null)
                predicate.And(t => t.CreatedAt < prms.BelowTime.Value.ToUniversalTime());

            if(!prms.IncludeDeleted && !prms.OnlyDeleted)
                predicate.And(t => t.DeletedAt == null);

            if(prms.OnlyDeleted)
                predicate.And(t => t.DeletedAt != null);

            if(prms.ByAccountId != 0)
                predicate.And(t => t.AccountId == prms.ByAccountId);

            return await _rep.Topic.Value
                .FindByCondition(predicate)
                .OrderByDescending(t => t.CreatedAt)
                .TakeOffset(offset)
                .Select(t => new {
                    Topic = t,
                    FirstPost = t.Posts
                        .Where(p => p.AncestorId == null)
                        .OrderBy(p => p.CreatedAt)
                        .First()
                })
                .Select(t => new TopicInfoResponse
                {
                    Topic = _mapper.Map<TopicDto>(t.Topic),
                    Sender = _mapper.Map<User>(t.Topic.Author),
                    Post = _mapper.Map<PostDto>(t.FirstPost),
                })
                .ToListAsync();
        }

        public async Task<List<TopicElement>> GetTopics(int forumId, Page page, Params prms)
        {
            var predicate = PredicateBuilder.New<Topic>(t => t.ForumId == forumId);
            if(prms.BelowTime != null)
                predicate.And(t => t.CreatedAt < prms.BelowTime.Value.ToUniversalTime());

            if(!prms.IncludeDeleted && !prms.OnlyDeleted)
                predicate.And(t => t.DeletedAt == null);

            if(prms.OnlyDeleted)
                predicate.And(t => t.DeletedAt != null);

            if(prms.ByAccountId != 0)
                predicate.And(t => t.AccountId == prms.ByAccountId);

            return await _rep.Topic.Value
                .FindByCondition(predicate)
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

        public async Task<TopicInfoResponse> Create(int authorId, TopicNew topicDto)
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
            return new  TopicInfoResponse{
                Topic = _mapper.Map<TopicDto>(topic),
                Post = _mapper.Map<PostDto>(post)
            };
        }

        public async Task<TopicDto> Update(int topicId, TopicEdit topicDto)
        {
            var entity = await _rep.Topic.Value
                .FindByCondition(t => t.Id == topicId && t.DeletedAt == null, true)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Topic not found");

            _mapper.Map(topicDto, entity);
            await _rep.Save();

            return _mapper.Map<TopicDto>(entity);
        }

        public async Task Delete(int topicId)
        {
            var topicEntity = await _rep.Topic.Value
                .FindByCondition(t => t.Id == topicId, true)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Topic not found");

            if(topicEntity.DeletedAt != null)
                throw new BadRequestException("Topic already deleted");

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

        public async Task<TopicDto> Recover(int topicId) 
        {
            var topic = await _rep.Topic.Value
                .FindByCondition(t => t.Id == topicId, true)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Topic not found");

            if(topic.DeletedAt == null)
                throw new BadRequestException("Topic not deleted");

            var deletedPosts = topic.Posts
                .Where(p => p.DeletedAt == topic.DeletedAt)
                .ToList();

            await _rep.BeginTransaction();
            try 
            {
                topic.DeletedAt = null;
                deletedPosts.ForEach(p => 
                {
                    p.DeletedAt = null;
                });

                await _rep.Save();
                await _rep.Commit();
            }
            catch
            {
                await _rep.Rollback();
                throw;
            }

            return _mapper.Map<TopicDto>(topic);
        }
    }
}