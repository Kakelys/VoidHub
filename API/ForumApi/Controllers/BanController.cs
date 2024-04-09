using ForumApi.Data.Models;
using ForumApi.DTO.DBan;
using ForumApi.DTO.Utils;
using ForumApi.Utils.Extensions;
using ForumApi.Controllers.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ForumApi.Services.Auth.Interfaces;
using SixLabors.ImageSharp.ColorSpaces.Companding;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/bans")]
    public class BanController(IBanService banService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetBans([FromQuery] Page page)
        {
            return Ok(await banService.GetBans(page));
        }

        [HttpPost]
        [Authorize(Roles = $"{Role.Admin},{Role.Moder}")]
        [BanFilter]
        public async Task<IActionResult> Create(BanEdit ban)
        {
            return Ok(await banService.Create(User.GetId(), ban));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{Role.Admin},{Role.Moder}")]
        [BanFilter]
        public async Task<IActionResult> Update(int id, BanEdit ban)
        {
            return Ok(await banService.Update(User.GetId(), id, ban));
        }

        [HttpDelete]
        [Authorize(Roles = $"{Role.Admin},{Role.Moder}")]
        [BanFilter]
        public async Task<IActionResult> Delete([FromQuery] string username)
        {
            Console.WriteLine("ban");
            await banService.Delete(User.GetId(), username);
            return Ok();
        }
    }
}