using AutoMapper;
using ForumApi.Data.Models;
using ForumApi.Data.Repository.Extensions;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.Auth;
using ForumApi.DTO.DPost;
using ForumApi.DTO.Page;
using ForumApi.Utils.Exceptions;
using ForumApi.Services.ForumS.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Services.ForumS
{
    public class PostService : IPostService
    {
        private readonly IRepositoryManager _rep;
        private readonly IMapper _mapper;

        public PostService(
            IRepositoryManager rep,
            IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }

        public async Task<Post> Create(int accountId, PostDto postDto)
        {
            if(!_rep.IsInTransaction)
                throw new DatabaseException("Function runs outside the transaction");

            var topicEntity = await _rep.Topic.Value
                .FindByCondition(t => t.Id == postDto.TopicId, true)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Topic not found");

            if(topicEntity.IsClosed)
                throw new BadRequestException("Topic is closed");

            if(topicEntity.DeletedAt != null)
                throw new BadRequestException("Topic currently deleted; creating new posts unavailable");

            var post = _mapper.Map<Post>(postDto);
            post.AccountId = accountId;

            var entity = _rep.Post.Value.Create(post);
            await _rep.Save();
        
            // update ancestors comments counter
            await _rep.Post.Value.IncreaseAllAncestorsCommentsCount(entity.AncestorId, 1);

            // upd topic posts counter, if it not comment
            if(postDto.AncestorId != null && entity.Ancestor.AncestorId == null)
                topicEntity.PostsCount++;

            await _rep.Save();

            return post;
        }

        public async Task Delete(int postId)
        {
            var entity = await _rep.Post.Value
                .FindByCondition(p => p.Id == postId && p.DeletedAt == null, true)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Post not found");

            var topicFirstPost = await _rep.Post.Value
                .FindByCondition(p => p.TopicId == entity.TopicId && p.DeletedAt == null)
                .OrderBy(p => p.CreatedAt)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Post not found");

            var topic = entity.Topic;

            if (entity.Id == topicFirstPost.Id)
                throw new BadRequestException("You can't delete main post");

            if(topic.DeletedAt != null)
                throw new BadRequestException("Topic currently deleted; deleting posts unavailable");

            await _rep.BeginTransaction();
            try
            {
                var deleted = _rep.Post.Value.Delete(entity);
                await _rep.Save();

                // update ancestors comments counter
                await _rep.Post.Value.IncreaseAllAncestorsCommentsCount(entity.AncestorId, -deleted);

                // upd topic posts counter, if it not comment
                if(entity.AncestorId == topicFirstPost.Id)
                {
                    topic.PostsCount--;
                }

                await _rep.Save();
                await _rep.Commit();
            }
            catch
            {
                await _rep.Rollback();
                throw;
            }
        }

        public async Task<List<PostResponse>> GetPostComments(int? ancestorId, Offset page, bool allowDeleted = false)
        {
            var posts = await _rep.Post.Value
                .FindByCondition(p => p.AncestorId == ancestorId)
                .AllowDeletedWithTopic(allowDeleted)
                .OrderBy(p => p.CreatedAt)
                .Include(p => p.Author)
                .TakeOffset(page)
                .Select(p => new PostResponse(p)
                {
                    Author = _mapper.Map<User>(p.Author)
                })
                .ToListAsync();

            return posts;
        }

        public async Task<Post> Update(int postId, PostDto postDto)
        {
            var entity = await _rep.Post.Value
                .FindByCondition(p => p.Id == postId && p.DeletedAt == null, true)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Post not found");

            if(entity.Topic.DeletedAt != null)
                throw new BadRequestException("Topic currently deleted; updating posts unavailable");

            entity.Content = postDto.Content;
            await _rep.Save();

            return entity;
        }
    }
}