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
using AspNetCore.Localizer.Json.Localizer;
using ForumApi.DTO.DForum;

namespace ForumApi.Services.ForumS
{
    public class TopicService(
        IMapper mapper, 
        IRepositoryManager rep, 
        IJsonStringLocalizer locale) : ITopicService
    {
        public async Task<TopicResponse?> GetTopic(int id, bool allowDeleted = false)
        {
            var firstPost = await rep.Post.Value
                .FindByCondition(p => p.TopicId == id)
                .AllowDeletedWithTopic(allowDeleted)
                .Include(p => p.Author)
                .OrderBy(p => p.CreatedAt)
                .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-topic"]);

            return await rep.Topic.Value
                .FindByCondition(t => t.Id == id)
                .AllowDeleted(allowDeleted)
                .Select(t => new TopicResponse
                {
                    Topic = mapper.Map<TopicDto>(t),
                    Post = mapper.Map<PostDto>(firstPost),
                    Sender = mapper.Map<User>(firstPost.Author),
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

            return await rep.Topic.Value
                .FindByCondition(predicate)
                .OrderByDescending(t => t.CreatedAt)
                .TakeOffset(offset)
                .Select(t => new {
                    Topic = t,
                    t.Forum,
                    FirstPost = t.Posts
                        .Where(p => p.AncestorId == null)
                        .OrderBy(p => p.CreatedAt)
                        .First()
                })
                .Select(t => new TopicInfoResponse
                {
                    Topic = mapper.Map<TopicDto>(t.Topic),
                    Sender = mapper.Map<User>(t.Topic.Author),
                    Forum = mapper.Map<ForumDto>(t.Forum),
                    Post = mapper.Map<PostDto>(t.FirstPost),
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

            return await rep.Topic.Value
                .FindByCondition(predicate)
                .OrderByDescending(t => t.IsPinned)
                .ThenByDescending(t => t.CreatedAt)
                .Select(t => new TopicElement(t)
                {
                    Author = mapper.Map<User>(t.Author),
                    LastPost = t.Posts
                        .Where(p => p.DeletedAt == null)
                        .OrderByDescending(p => p.CreatedAt)
                        .Select(p => new LastPost
                        {
                            Id = p.Id,
                            CreatedAt = p.CreatedAt,
                            Author = mapper.Map<User>(p.Author)
                        }).FirstOrDefault()
                })
                .TakePage(page)
                .ToListAsync();
        }

        public async Task<TopicInfoResponse> Create(int authorId, TopicNew topicDto)
        {
            var forum = await rep.Forum.Value
                .FindByCondition(f => f.Id == topicDto.ForumId)
                .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-forum"]);

            if(forum.IsClosed)
                throw new BadRequestException(locale["errors.forum-closed"]);

            var topic = mapper.Map<Topic>(topicDto);
            topic.AccountId = authorId;

            var post = new Post
            {
                Content = topicDto.Content, 
                AccountId = authorId
            };

            topic.Posts.Add(post);

            rep.Topic.Value.Create(topic);
            await rep.Save();
            return new  TopicInfoResponse{
                Topic = mapper.Map<TopicDto>(topic),
                Post = mapper.Map<PostDto>(post)
            };
        }

        public async Task<TopicDto> Update(int topicId, TopicEdit topicDto)
        {
            var entity = await rep.Topic.Value
                .FindByCondition(t => t.Id == topicId && t.DeletedAt == null, true)
                .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-topic"]);

            mapper.Map(topicDto, entity);
            await rep.Save();

            return mapper.Map<TopicDto>(entity);
        }

        public async Task Delete(int topicId)
        {
            var topicEntity = await rep.Topic.Value
                .FindByCondition(t => t.Id == topicId, true)
                .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-topic"]);

            if(topicEntity.DeletedAt != null)
                throw new BadRequestException(locale["errors.topic-already-deleted"]);

            rep.Topic.Value.Delete(topicEntity);

            // also delete posts
            // use same time as in topic, so it can be easy recover after
            rep.Post.Value.DeleteMany(
                rep.Post.Value
                    .FindByCondition(p => p.TopicId == topicEntity.Id && p.DeletedAt == null, true)
                    .ToList(), 
                topicEntity.DeletedAt
            );

            await rep.Save();
        }

        public async Task<TopicDto> Recover(int topicId) 
        {
            var topic = await rep.Topic.Value
                .FindByCondition(t => t.Id == topicId, true)
                .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-topic"]);

            if(topic.DeletedAt == null)
                throw new BadRequestException(locale["errors.topic-not-deleted"]);

            var deletedPosts = topic.Posts
                .Where(p => p.DeletedAt == topic.DeletedAt)
                .ToList();

            await rep.BeginTransaction();
            try 
            {
                topic.DeletedAt = null;
                deletedPosts.ForEach(p => 
                {
                    p.DeletedAt = null;
                });

                await rep.Save();
                await rep.Commit();
            }
            catch
            {
                await rep.Rollback();
                throw;
            }

            return mapper.Map<TopicDto>(topic);
        }
    }
}