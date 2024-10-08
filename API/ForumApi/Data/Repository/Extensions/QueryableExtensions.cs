using ForumApi.DTO.Utils;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Data.Repository.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> EnableAsTracking<T>(this IQueryable<T> query, bool asTracking)
        where T : class =>
        asTracking ? query.AsTracking() : query.AsNoTracking();

    public static IQueryable<T> TakePage<T>(this IQueryable<T> query, Page page)
        where T : class =>
        query.Skip((page.PageNumber - 1) * page.PageSize)
        .Take(page.PageSize);

    public static IQueryable<T> TakeOffset<T>(this IQueryable<T> query, Offset offset)
        where T : class =>
        query.Skip(offset.OffsetNumber)
        .Take(offset.Limit);
}