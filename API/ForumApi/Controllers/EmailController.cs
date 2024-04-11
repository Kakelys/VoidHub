using ForumApi.Controllers.Filters;
using ForumApi.Services.Email.Interfaces;
using ForumApi.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/email")]
    public class EmailController(IConfirmService confirmService) : ControllerBase
    {

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string base64Token)
        {
            await confirmService.ConfirmEmail(base64Token);
            return Ok();
        }

        [HttpGet("resend-confirm")]
        [Authorize]
        [BanFilter]
        public async Task<IActionResult> ResendConfirmEmail()
        {
            await confirmService.SendConfirmEmail(User.GetId());
            return Ok();
        }

    }
}