using AutoMapper;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.DForum;
using ForumApi.DTO.DSection;
using ForumApi.DTO.DTopic;
using ForumApi.DTO.Utils;
using ForumApi.Utils.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/breadcrumbs")]
    public class BreadcrumbController(
        IRepositoryManager rep,
        IMapper mapper) : ControllerBase
    {
        [HttpGet("from-forum")]
        public async Task<IActionResult> FromForum([FromQuery] int forumId)
        {
            var tmp = await rep.Forum.Value
                .FindByCondition(f => f.Id == forumId)
                .Include(f => f.Section)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("");

            var links = new List<Link>
            {
                new() {Type = "section", Data = mapper.Map<SectionDto>(tmp.Section)},
                new() {Type = "forum", Data = mapper.Map<ForumDto>(tmp)},
            };

            return Ok(links);
        }

        [HttpGet("from-topic")]
        public async Task<IActionResult> FromTopic([FromQuery] int topicId)
        {
            var tmp = await rep.Topic.Value
                .FindByCondition(t => t.Id == topicId)
                .Include(t => t.Forum.Section)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("");

            var links = new List<Link>
            {
                new() {Type = "section", Data = mapper.Map<SectionDto>(tmp.Forum.Section)},
                new() {Type = "forum", Data = mapper.Map<ForumDto>(tmp.Forum)},
                new() {Type = "topic", Data = mapper.Map<TopicDto>(tmp)},
            };

            return Ok(links);
        }
    }
}