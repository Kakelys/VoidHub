using ForumApi.DTO.DSearch;
using ForumApi.DTO.Utils;
using ForumApi.Services.ForumS.Interfaces;
using ForumApi.Services.Utils.Interfaces;
using ForumApi.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/search")]
    public class SearchController(
        ISearchService searchService,
        ILikeService likeService) : ControllerBase
    {
        [Authorize]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] SearchDto search, [FromQuery] SearchParams searchParams, [FromQuery] Page page)
        {
            var res = new SearchResponse<object>();
            var prms = new SearchParams
            {
                Sort = searchParams.Sort,
                WithPostContent = searchParams.WithPostContent,
                PartialTitle = searchParams.PartialTitle
            };

            if(User.IsAdminOrModer())
            {
                prms.OnlyDeleted = searchParams.OnlyDeleted;
            }

            var forSwitch = search.Query[0..2].ToLower();
            search.Query = search.Query.Trim();
            switch(forSwitch)
            {
                case "t:":
                    search.Query = search.Query[2..];
                    await TopicSearch(res, search, prms, page);
                    break;
                case "u:":
                    search.Query = search.Query[2..];
                    await UserSearch(res, search, prms, page);
                    break;
                default:
                    await TopicSearch(res, search, prms, page);
                    UpdPage(res.SearchCount, res.Data.Count, page);
                    await UserSearch(res, search, prms, page);
                    break;
            }

            return Ok(res);
        }

        private async Task TopicSearch(SearchResponse<object> res, SearchDto search, SearchParams prms, Page page)
        {
            search.Query = search.Query.Trim();
            var topicRes = await searchService.SearchTopics(search.Query, prms, page);
            if(User.IsAuthed())
            {
                var userId = User.GetId();
                foreach(var post in topicRes.Data)
                {
                    await likeService.UpdateLikeStatus(userId, post.Post);
                }
            }
            res.SearchCount += topicRes.SearchCount;
            res.Data.AddRange(topicRes.Data.ConvertAll(t => new SearchElement
            {
                Type = SearchElementType.Topic,
                Data = t
            }));
        }

        private async Task UserSearch(SearchResponse<object> res, SearchDto search, SearchParams prms, Page page)
        {
            search.Query = search.Query.Trim();
            var userRes = await searchService.SearchUsers(search.Query, prms, page);

            res.SearchCount += userRes.SearchCount;
            res.Data.AddRange(userRes.Data.ConvertAll(t => new SearchElement
            {
                Type = SearchElementType.User,
                Data = t
            }));
        }

        private static void UpdPage(int searchCount, int loaded, Page page)
        {
            if(searchCount >= page.PageSize && searchCount != loaded)
            {
                page.PageNumber -= searchCount / page.PageSize;
                if(searchCount % page.PageSize != 0)
                    page.PageSize -= searchCount % page.PageSize;
            }
            else
            {
                page.PageSize -= loaded;
            }

            if(page.PageNumber <= 0)
            {
                page.PageSize = 0;
                page.PageNumber = 1;
            }
        }
    }
}