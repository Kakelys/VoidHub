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

namespace ForumApi.Services.Utils
{
    public class SearchService(IRepositoryManager rep, IMapper mapper) : ISearchService
    {
        public async Task<SearchResponse> SearchTopics(string query, SearchParams search, Page page)
        {
            query = query.Trim();

            var basePredicate = PredicateBuilder.New<Topic>(t => search.OnlyDeleted ? t.DeletedAt != null : t.DeletedAt == null);
            var orPredicate = PredicateBuilder.New<Topic>(t => true);

            // configure tsquery search
            var forTsQuery = query.Split(' ')
                .Select(w => $"{w}:*")
                .Aggregate((a, b) => $"{a} | {b}");

            var predicators = new Dictionary<string, Expression<Func<Topic, bool>>>
            {
                [SearchParamNames.WordTitle] = t => t.SearchVector.Matches(EF.Functions.ToTsQuery("english", forTsQuery)),
                [SearchParamNames.WordContent] = t => t.Posts.Where(p => p.AncestorId == null).First().SearchVector.Matches(EF.Functions.ToTsQuery("english", forTsQuery)),
                [SearchParamNames.PartialTitle] = t => EF.Functions.ILike(t.Title, $"%{query}%"),
                [SearchParamNames.PartialContent] = t => EF.Functions.ILike(t.Posts.OrderByDescending(p => p.CreatedAt).First().Content ?? "", $"%{query}%"),
            };

            orPredicate.And(predicators[SearchParamNames.WordTitle]);

            if(search.WithPostContent)
                orPredicate.Or(predicators[SearchParamNames.WordContent]);

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

            var searchRes = new SearchResponse
            {
                SearchCount = q.Count(),
                Data = await q.TakePage(page).Select(t => new TopicInfoResponse 
                {
                    Sender = mapper.Map<User>(t.Author),
                    Topic = mapper.Map<TopicDto>(t),
                    Post = mapper.Map<PostDto>(t.Posts.OrderByDescending(p => p.CreatedAt).First())
                }).ToListAsync()
            };
            
            return searchRes;
        }
    }
}