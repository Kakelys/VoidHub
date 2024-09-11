using System.Linq.Dynamic.Core;
using ForumApi.Data.Models;

namespace ForumApi.DTO.DSearch.Sort;

public static class SearchSort
{
    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, SearchElementType searchType, SearchSortTypes sortType)
    {
        switch (searchType)
        {
            case SearchElementType.Topic:
                return (IQueryable<T>)ForTopic((IQueryable<Topic>)query, sortType);
            case SearchElementType.User:
                return (IQueryable<T>)ForUser((IQueryable<Account>)query, sortType);
            default:
                return query.OrderBy("CreatedAt desc");
        }
    }

    private static IQueryable<Topic> ForTopic(IQueryable<Topic> query, SearchSortTypes sortType)
    {
        return sortType switch
        {
            SearchSortTypes.Old => query.OrderBy(t => t.CreatedAt),
            SearchSortTypes.MostLiked => query.OrderByDescending(t =>
                t.Posts
                .OrderBy(p => p.CreatedAt)
                .First().LikesCount
            ),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };
    }

    private static IQueryable<Account> ForUser(IQueryable<Account> query, SearchSortTypes sortType)
    {
        return sortType switch
        {
            SearchSortTypes.MostLiked => query.OrderByDescending(t => t.Posts.Sum(p => p.LikesCount)),
            SearchSortTypes.Old => query.OrderBy(t => t.CreatedAt),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };
    }
}