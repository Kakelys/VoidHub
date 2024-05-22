using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using ForumApi.Data.Models;
using ForumApi.Data.Repository.Extensions;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.DSearch;
using ForumApi.DTO.Utils;
using ForumApi.Services.Utils.Interfaces;
using Microsoft.EntityFrameworkCore;
using LinqKit;
using ForumApi.DTO.DTopic;
using AutoMapper;
using ForumApi.DTO.Auth;
using ForumApi.DTO.DPost;
using ForumApi.DTO.DForum;
using ForumApi.DTO.DSearch.Sort;

namespace ForumApi.Services.Utils
{
    public class SearchService(IRepositoryManager rep, IMapper mapper) : ISearchService
    {
        public async Task<SearchResponse<TopicInfoResponse>> SearchTopics(string query, SearchParams search, Page page)
        {
            var basePredicate = PredicateBuilder.New<Topic>(t => search.OnlyDeleted ? t.DeletedAt != null : t.DeletedAt == null);
            var orPredicate = PredicateBuilder.New<Topic>(true);

            // configure tsquery search
            var forTsQuery = "";

            query = query.Replace(":", "");

            if(!string.IsNullOrEmpty(query))
            {
                forTsQuery = query.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .Select(w => $"{w}:*")
                    .Aggregate((a, b) => $"{a} | {b}");
            }

            orPredicate.And(t => t.SearchVector.Matches(EF.Functions.ToTsQuery("english", forTsQuery)));

            if(search.WithPostContent)
                orPredicate.Or(t => t.Posts.OrderByDescending(p => p.CreatedAt).First(p => p.AncestorId == null).SearchVector.Matches(EF.Functions.ToTsQuery("english", forTsQuery)));

            if(search.PartialTitle)
                orPredicate.Or(t => EF.Functions.ILike(t.Title, $"%{query}%"));

            if(search.ForumId > 0)
                basePredicate.And(f => f.ForumId == search.ForumId);

            // do search
            var q = rep.Topic.Value.FindByCondition(basePredicate, true).Where(orPredicate);

            q = q.OrderByDescending(t => t.SearchVector.Rank(
                EF.Functions.ToTsQuery("english", forTsQuery)
            ));

            //apply sort
            q = q.ApplySort(SearchElementType.Topic, search.Sort);

            return new SearchResponse<TopicInfoResponse>
            {
                SearchCount = q.Count(),
                Data = await q.TakePage(page).Select(t => new TopicInfoResponse
                {
                    Sender = mapper.Map<User>(t.Author),
                    Topic = mapper.Map<TopicDto>(t),
                    Forum = mapper.Map<ForumDto>(t.Forum),
                    Post = mapper.Map<PostDto>(t.Posts.OrderBy(p => p.CreatedAt).First())
                }).ToListAsync()
            };
        }

        public async Task<SearchResponse<User>> SearchUsers(string query, SearchParams search, Page page)
        {
            var predicate = PredicateBuilder.New<Account>(t => search.OnlyDeleted ? t.DeletedAt != null : t.DeletedAt == null);
            predicate.And(a => EF.Functions.ILike(a.Username, $"%{query}%"));

            var q = rep.Account.Value
                .FindByCondition(predicate);

            //apply sort
            q = q.ApplySort(SearchElementType.User, search.Sort);

            return new()
            {
                SearchCount = q.Count(),
                Data = await q.TakePage(page).Select(a => mapper.Map<User>(a)).ToListAsync()
            };
        }
    }
}