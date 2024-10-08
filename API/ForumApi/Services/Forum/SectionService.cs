using AutoMapper;
using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.Auth;
using ForumApi.DTO.DForum;
using ForumApi.DTO.DSection;
using ForumApi.DTO.DTopic;
using ForumApi.Utils.Exceptions;
using ForumApi.Services.ForumS.Interfaces;
using Microsoft.EntityFrameworkCore;
using LinqKit;
using AspNetCore.Localizer.Json.Localizer;

namespace ForumApi.Services.ForumS;

public class SectionService(
    IRepositoryManager repositoryManager,
    IMapper mapper,
    IJsonStringLocalizer locale
) : ISectionService
{

    private readonly IRepositoryManager _rep = repositoryManager;
    private readonly IMapper _mapper = mapper;

    public async Task<List<SectionResponse>> GetSections(bool includeHidden = false)
    {
        var predicate = PredicateBuilder.New<Section>(s => true);
        if (!includeHidden)
            predicate.And(s => !s.IsHidden);

        return await _rep.Section.Value
            .FindByCondition(predicate)
            .OrderBy(s => s.OrderPosition)
            .Select(s => new SectionResponse
            {
                Section = _mapper.Map<SectionDto>(s),
                Forums = s.Forums
                    .Where(f => includeHidden || f.DeletedAt == null)
                    .Select(f => new
                    {
                        Forum = f,
                        Topics = f.Topics.Where(t => t.DeletedAt == null)
                    })
                    .Select(ff => new ForumResponse
                    {
                        Forum = _mapper.Map<ForumDto>(ff.Forum),
                        TopicsCount = ff.Topics.Count(),
                        PostsCount = ff.Topics.SelectMany(t => t.Posts).Count(p => p.DeletedAt == null),
                        LastTopic = ff.Topics
                            .OrderByDescending(t => t.Posts.Where(p => p.DeletedAt == null).Max(p => p.CreatedAt))
                            .Select(t => new
                            {
                                Topic = t,
                                Posts = t.Posts.Where(p => p.DeletedAt == null)
                                    .OrderByDescending(p => p.CreatedAt)
                                    .Select(p => p)
                            })
                            .Select(t => new TopicLast
                            {
                                Id = t.Topic.Id,
                                Title = t.Topic.Title,
                                UpdatedAt = t.Posts
                                    .First().CreatedAt,
                                User = _mapper.Map<User>(t.Posts
                                    .Select(p => p.Author)
                                    .FirstOrDefault())
                            }).FirstOrDefault()
                    }).ToList()
            }).ToListAsync();
    }

    public async Task<List<SectionDtoResponse>> GetDtoSections(bool includeHidden = false)
    {
        var predicate = PredicateBuilder.New<Section>(s => true);

        if (!includeHidden)
        {
            predicate.And(s => !s.IsHidden);
        }

        return await _rep.Section.Value
            .FindByCondition(predicate)
            .Include(s => s.Forums)
            .Select(s => new SectionDtoResponse
            {
                Section = _mapper.Map<SectionDto>(s),
                Forums = _mapper.Map<List<ForumDto>>(s.Forums)
            }).ToListAsync();
    }

    public async Task<Section> Create(SectionEdit section)
    {
        var newSection = _rep.Section.Value.Create(_mapper.Map<Section>(section));

        await _rep.Save();

        return newSection;
    }

    public async Task<Section> Update(int sectionId, SectionEdit section)
    {
        var entity = await _rep.Section.Value
            .FindByCondition(s => s.Id == sectionId, true)
            .FirstOrDefaultAsync()
                ?? throw new NotFoundException(locale["errors.no-section"]);

        _mapper.Map(section, entity);
        await _rep.Save();

        return entity;
    }

    public async Task Delete(int sectionId)
    {
        var entity = await _rep.Section.Value
            .FindByCondition(s => s.Id == sectionId, true)
            .FirstOrDefaultAsync()
                ?? throw new NotFoundException(locale["errors.no-section"]);

        if (entity.Forums.Count > 0)
        {
            throw new NotFoundException(locale["errors.no-section-with-forum-delete"]);
        }

        _rep.Section.Value.Delete(entity);
        await _rep.Save();
    }
}