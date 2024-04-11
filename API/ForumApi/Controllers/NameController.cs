using ForumApi.Data.Models;
using ForumApi.DTO.DName;
using ForumApi.Services.ForumS.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumApi.Controllers
{
    [Route("api/v1/names")]
    public class NameController(INamesService namesService) : ControllerBase
    {
        [HttpGet("section")]
        [Authorize(Roles = $"{Role.Admin}, {Role.Moder}")]
        public async Task<IActionResult> GetSectionNames()
        {
            return Ok(await namesService.GetSections());
        }

        [HttpGet("forum")]
        [Authorize]
        [AllowAnonymous]
        public async Task<IActionResult> GetForumNames()
        {
            List<Name>? res = null;
            if(User.Identity?.IsAuthenticated == true && (User.IsInRole(Role.Admin) || User.IsInRole(Role.Moder)))
            {
                res = await namesService.GetForums(true);
            }

            if(res == null)
                res = await namesService.GetForums();

            return Ok(res);
        }
    }
}