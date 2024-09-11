using AutoMapper;
using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.DForum;
using ForumApi.Utils.Exceptions;
using Microsoft.EntityFrameworkCore;
using ForumApi.Services.ForumS.Interfaces;
using ForumApi.DTO.Utils;
using AspNetCore.Localizer.Json.Localizer;
using LinqKit;

namespace ForumApi.Services.ForumS;

public class ForumService(
    IRepositoryManager rep,
    IMapper mapper,
    IJsonStringLocalizer locale
) : IForumService
{
    public async Task<ForumResponse> Get(int forumId, Params prms)
    {
        var predicate = PredicateBuilder.New<Forum>(f => f.Id == forumId);
        if (!prms.IncludeDeleted)
        {
            predicate.And(f => f.DeletedAt == null);
        }

        return await rep.Forum.Value
            .FindByCondition(predicate)
            .Select(f => new ForumResponse
            {
                Forum = mapper.Map<ForumDto>(f),
                PostsCount = f.Topics
                    .Where(t => prms.OnlyDeleted ? t.DeletedAt != null : t.DeletedAt == null)
                    .SelectMany(t => t.Posts)
                    .Count(p => p.DeletedAt == null),
                TopicsCount = f.Topics
                    .Count(t => prms.OnlyDeleted ? t.DeletedAt != null : t.DeletedAt == null)
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Forum> Create(ForumEdit forumDto)
    {
        var forum = rep.Forum.Value.Create(mapper.Map<Forum>(forumDto));
        await rep.Save();

        return forum;
    }

    public async Task<Forum> Update(int forumId, ForumEdit forumDto, string newImagePath)
    {
        var entity = await rep.Forum.Value
            .FindByCondition(f => f.Id == forumId, true)
            .FirstOrDefaultAsync()
                ?? throw new NotFoundException(locale["errors.no-forum"]);

        mapper.Map(forumDto, entity);
        if (!string.IsNullOrEmpty(newImagePath))
        {
            entity.ImagePath = newImagePath;
        }

        await rep.Save();

        return entity;
    }

    public async Task Delete(int forumId)
    {
        var entity = await rep.Forum.Value
            .FindByCondition(f => f.Id == forumId, true)
            .FirstOrDefaultAsync()
                ?? throw new NotFoundException(locale["errors.no-forum"]);

        if (entity.Topics.Count > 0)
        {
            throw new BadRequestException(locale["errors.delete-forums-with-topics"]);
        }

        rep.Forum.Value.Delete(entity);
        await rep.Save();
    }
}