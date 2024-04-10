using AspNetCore.Localizer.Json.Localizer;
using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.DPost;
using ForumApi.Services.ForumS.Interfaces;
using ForumApi.Utils.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Services.ForumS
{
    public class LikeService(
        IRepositoryManager rep,
        IJsonStringLocalizer locale) : ILikeService
    {
        public async Task UpdateLikeStatus(int accountId, PostDto post)
        {
            var isLiked = await rep.Like.Value
                .FindByCondition(l => l.AccountId == accountId && l.PostId == post.Id)
                .AnyAsync();

            post.IsLiked = isLiked;
        }

        public async Task UpdateLikeStatus(int accountId, List<PostDto> posts)
        {
            var tasks = new List<Task>(posts.Count);

            foreach(var post in posts)
            {
                tasks.Add(Task.Run(async () => 
                {
                    await UpdateLikeStatus(accountId, post);
                }));
            }

            await Task.WhenAll(tasks);
        }

        public async Task Like(int accountId, int postId)
        {
            var user = await rep.Account.Value
                .FindById(accountId)
                .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-user"]);

            var post = await rep.Post.Value
                .FindByCondition(p => p.Id == postId && p.DeletedAt == null, true)
                .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-post"]);

            if(await rep.Like.Value.FindByCondition(l => l.AccountId == accountId && l.PostId == postId).AnyAsync())
                return;

            post.LikesCount++;
            var like = new Like()
            {
                AccountId = accountId,
                PostId = postId
            };

            rep.Like.Value.Create(like);
            await rep.Save();
        }

        public async Task UnLike(int accountId, int postId)
        {
            var user = await rep.Account.Value
                .FindById(accountId)
                .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-user"]);

            var post = await rep.Post.Value
                .FindByCondition(p => p.Id == postId && p.DeletedAt == null, true)
                .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-post"]);

            var like = await rep.Like.Value.FindByCondition(l => l.AccountId == accountId && l.PostId == postId)
                .FirstOrDefaultAsync();
            if(like == null)
                return;

            post.LikesCount--;

            rep.Like.Value.Delete(like);
            await rep.Save();
        }
    }
}