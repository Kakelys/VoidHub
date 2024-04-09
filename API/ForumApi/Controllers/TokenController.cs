using ForumApi.Data.Models;
using ForumApi.Controllers.Filters;
using ForumApi.Services.Auth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/tokens")]
    public class TokenController(ITokenService tokenService) : ControllerBase
    {
        [HttpDelete("{token}")]
        [Authorize]
        [PermissionActionFilter<Token>]
        public async Task<IActionResult> Revoke(string token)
        {
            await tokenService.Revoke(token);
            return Ok();
        }
    }
}