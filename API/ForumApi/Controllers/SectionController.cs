using ForumApi.Data.Models;
using ForumApi.DTO.DSection;
using ForumApi.Controllers.Filters;
using ForumApi.Services.ForumS.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/sections")]
    public class SectionController : ControllerBase
    {
        private readonly ISectionService _sectionService;

        public SectionController(
            ISectionService sectionService)
        {
            _sectionService = sectionService;
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            List<SectionResponse> res = null;
            if(User.Identity != null && User.Identity.IsAuthenticated)
            {
                if(User.IsInRole(Role.Admin))
                    res = await _sectionService.GetSections(true);
            }
            
            if(res == null)
                res = await _sectionService.GetSections();
            
            return Ok(res);
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> Create(SectionEdit sectionDto)
        {
            var section = await _sectionService.Create(sectionDto);
            return Ok(section);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> Update(int id, SectionEdit sectionDto)
        {
            var section = await _sectionService.Update(id, sectionDto);
            return Ok(section);
        }
    }
}