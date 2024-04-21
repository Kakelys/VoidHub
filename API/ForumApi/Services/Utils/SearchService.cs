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
using ForumApi.DTO.DAccount;

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

            if(!string.IsNullOrEmpty(query))
            {
                forTsQuery = query.Split(' ')
                    .Select(w => $"{w}:*")
                    .Aggregate((a, b) => $"{a} | {b}");
            }

            var predicators = new Dictionary<string, Expression<Func<Topic, bool>>>
            {
                [SearchParamNames.WordTitle] = t => t.SearchVector.Matches(EF.Functions.ToTsQuery("english", forTsQuery)),
                [SearchParamNames.WordContent] = t => t.Posts.First(p => p.AncestorId == null).SearchVector.Matches(EF.Functions.ToTsQuery("english", forTsQuery)),
                [SearchParamNames.PartialTitle] = t => EF.Functions.ILike(t.Title, $"%{query}%"),
                [SearchParamNames.PartialContent] = t => EF.Functions.ILike(t.Posts.OrderByDescending(p => p.CreatedAt).First().Content ?? "", $"%{query}%"),
            };

            orPredicate.And(predicators[SearchParamNames.WordTitle]);

            if(search.WithPostContent)
                orPredicate.Or(predicators[SearchParamNames.WordContent]);

            if(search.PartialTitle)
                orPredicate.Or(predicators[SearchParamNames.PartialTitle]);

            // do search
            var q = rep.Topic.Value.FindByCondition(basePredicate, true).Where(orPredicate);

            q = q.OrderByDescending(t => t.SearchVector.Rank(
                EF.Functions.ToTsQuery("english", forTsQuery)
            ));

            //apply sort
            if(string.IsNullOrEmpty(search.Sort))
            {
                q = ((IOrderedQueryable<Topic>)q).ThenByDescending(t => t.CreatedAt);
            }
            else
            {
                //TODO: replace with normal))
                q = ((IOrderedQueryable<Topic>)q).ThenBy($"CreatedAt {search.Sort}");
            }

            return new SearchResponse<TopicInfoResponse>
            {
                SearchCount = q.Count(),
                Data = await q.TakePage(page).Select(t => new TopicInfoResponse
                {
                    Sender = mapper.Map<User>(t.Author),
                    Topic = mapper.Map<TopicDto>(t),
                    Post = mapper.Map<PostDto>(t.Posts.OrderByDescending(p => p.CreatedAt).First())
                }).ToListAsync()
            };
        }

        public async Task<SearchResponse<User>> SearchUsers(string query, SearchParams search, Page page)
        {
            var predicate = PredicateBuilder.New<Account>(t => search.OnlyDeleted ? t.DeletedAt != null : t.DeletedAt == null);
            predicate.And(a => EF.Functions.ILike(a.Username, $"%{query}%"));

            var q = rep.Account.Value
                .FindByCondition(predicate);

            return new()
            {
                SearchCount = q.Count(),
                Data = await q.TakePage(page).Select(a => mapper.Map<User>(a)).ToListAsync()
            };
        }
    }
}