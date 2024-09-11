using AutoMapper;
using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.Auth;
using ForumApi.DTO.DPost;
using ForumApi.DTO.Utils;
using ForumApi.Utils.Exceptions;
using ForumApi.Services.ForumS.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using ForumApi.DTO.DTopic;
using LinqKit;
using ForumApi.Data.Repository.Extensions;
using AspNetCore.Localizer.Json.Localizer;
using ForumApi.DTO.DForum;

namespace ForumApi.Services.ForumS;

public class PostService(
    IRepositoryManager rep,
    IMapper mapper,
    IJsonStringLocalizer locale
) : IPostService
{
    public async Task<Post> Create(int accountId, PostEditDto postDto)
    {
        if (!rep.IsInTransaction)
        {
            throw new DatabaseException(locale["errors.out-of-transaction"]);
        }

        var topicEntity = await rep.Topic.Value
            .FindByCondition(t => t.Id == postDto.TopicId, true)
            .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-topic"]);

        if (topicEntity.IsClosed)
        {
            throw new BadRequestException(locale["errors.topic-closed"]);
        }

        if (topicEntity.DeletedAt != null)
        {
            throw new BadRequestException($"{locale["errors.topic-deleted"]} {locale["errors.creating-post-unavailable"]}");
        }

        var post = mapper.Map<Post>(postDto);
        post.AccountId = accountId;

        var entity = rep.Post.Value.Create(post);
        await rep.Save();

        // update ancestors comments counter
        await rep.Post.Value.IncreaseAllAncestorsCommentsCount(entity.AncestorId, 1);

        // upd topic posts counter, if it's not a comment
        if (postDto.AncestorId != null && entity.Ancestor?.AncestorId == null)
        {
            topicEntity.PostsCount++;
        }

        await rep.Save();

        return post;
    }

    public async Task Delete(int postId)
    {
        var entity = await rep.Post.Value
            .FindByCondition(p => p.Id == postId && p.DeletedAt == null, true)
            .FirstOrDefaultAsync()
                ?? throw new NotFoundException(locale["errors.no-post"]);

        var topicFirstPost = await rep.Post.Value
            .FindByCondition(p => p.TopicId == entity.TopicId && p.DeletedAt == null)
            .OrderBy(p => p.CreatedAt)
            .FirstOrDefaultAsync()
                ?? throw new NotFoundException(locale["errors.no-post"]);

        var topic = entity.Topic;

        if (entity.Id == topicFirstPost.Id)
        {
            throw new BadRequestException(locale["errors.delete-main-post"]);
        }

        if (topic.DeletedAt != null)
        {
            throw new BadRequestException($"{locale["errors.topic-deleted"]}; {locale["errors.deleting-post-inavailable"]}");
        }

        await rep.BeginTransaction();
        try
        {
            var deleted = rep.Post.Value.Delete(entity);
            await rep.Save();

            // update ancestors comments counter
            await rep.Post.Value.IncreaseAllAncestorsCommentsCount(entity.AncestorId, -deleted);

            // upd topic posts counter, if it not comment
            if (entity.AncestorId == topicFirstPost.Id)
            {
                topic.PostsCount--;
            }

            await rep.Save();
            await rep.Commit();
        }
        catch
        {
            await rep.Rollback();
            throw;
        }
    }

    public async Task<List<PostResponse>> GetPostComments(int? ancestorId, Offset page, Params prms)
    {
        var predicate = PredicateBuilder.New<Post>(p => p.AncestorId == ancestorId);
        if (prms.BelowTime != null)
        {
            predicate.And(p => p.CreatedAt < prms.BelowTime.Value.ToUniversalTime());
        }

        if (prms.ByAccountId != 0)
        {
            predicate.And(p => p.AccountId == prms.ByAccountId);
        }

        var posts = await rep.Post.Value
            .FindByCondition(predicate)
            .AllowDeletedWithTopic(prms.IncludeDeleted)
            .OrderBy(p => p.CreatedAt)
            .Include(p => p.Author)
            .TakeOffset(page)
            .Select(p => new PostResponse
            {
                Post = mapper.Map<PostDto>(p),
                Sender = mapper.Map<User>(p.Author)
            })
            .ToListAsync();

        return posts;
    }

    public async Task<List<PostInfoResponse>> GetPosts(Offset offset, Params prms)
    {
        var predicate = PredicateBuilder.New<Post>(p => true);

        if (prms.BelowTime != null)
        {
            predicate.And(t => t.CreatedAt < prms.BelowTime.Value.ToUniversalTime());
        }

        if (!prms.IncludeDeleted)
        {
            predicate.And(t => t.DeletedAt == null);
        }

        if (prms.ByAccountId != 0)
        {
            predicate.And(t => t.AccountId == prms.ByAccountId);
        }

        return await rep.Post.Value
            .FindByCondition(predicate)
            .OrderBy(prms.OrderBy)
            .Include(p => p.Author)
            .Include(p => p.Topic)
            .TakeOffset(offset)
            .Select(p => new PostInfoResponse
            {
                Post = mapper.Map<PostDto>(p),
                Topic = mapper.Map<TopicDto>(p.Topic),
                Forum = mapper.Map<ForumDto>(p.Topic.Forum),
                Sender = mapper.Map<User>(p.Author)
            })
            .ToListAsync();
    }

    public async Task<Post> Update(int postId, PostEditDto postDto)
    {
        var entity = await rep.Post.Value
            .FindByCondition(p => p.Id == postId && p.DeletedAt == null, true)
            .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-post"]);

        if (entity.Topic.DeletedAt != null)
        {
            throw new BadRequestException($"{locale["errors.topic-deleted"]}; {locale["errors.updating-post-unavailable"]} ");
        }

        entity.Content = postDto.Content;
        await rep.Save();

        return entity;
    }
}