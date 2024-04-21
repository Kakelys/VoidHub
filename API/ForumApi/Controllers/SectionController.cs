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
    public class SectionController(
        ISectionService sectionService) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            if(User.Identity?.IsAuthenticated == true && User.IsInRole(Role.Admin))
                return Ok(await sectionService.GetSections(true));
            else
                return Ok(await sectionService.GetSections());
        }

        [HttpGet("short")]
        [Authorize]
        [AllowAnonymous]
        public async Task<IActionResult> GetShort()
        {
            if(User.Identity?.IsAuthenticated == true && User.IsInRole(Role.Admin))
                return Ok(await sectionService.GetDtoSections(true));
            else
                return Ok(await sectionService.GetDtoSections());
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> Create(SectionEdit sectionDto)
        {
            var section = await sectionService.Create(sectionDto);
            return Ok(section);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> Update(int id, SectionEdit sectionDto)
        {
            var section = await sectionService.Update(id, sectionDto);
            return Ok(section);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Admin)]
        [BanFilter]
        public async Task<IActionResult> Delete(int id)
        {
            await sectionService.Delete(id);
            return Ok();
        }
    }
}