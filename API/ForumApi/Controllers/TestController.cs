using ForumApi.Data.Repository.Interfaces;
using ForumApi.Services.Email.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("tmp")]
    public class TestController(IConfirmService cs, IRepositoryManager rep) : ControllerBase
    {
        [HttpGet("send-confirm")]
        public async Task<IActionResult> TestConfirmSend([FromQuery] int accountId)
        {
            await cs.SendConfirmEmail(accountId);
            return Ok();
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> TestConfirm([FromQuery] string base64Token)
        {
            await cs.ConfirmEmail(base64Token);
            return Ok();
        }

        [HttpGet("off-confirm")]
        public async Task<IActionResult> TestOffConfirm([FromQuery] int accountId)
        {
            var e = rep.Account.Value.FindById(accountId, true).FirstOrDefault();
            e.IsEmailConfirmed = false;
            await rep.Save();
            return Ok();
        }
    }
}