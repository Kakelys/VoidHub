using ForumApi.Controllers.Filters;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.Auth;
using ForumApi.Services.Email.Interfaces;
using ForumApi.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/email")]
    public class EmailController(
        IConfirmService confirmService,
        IPasswordRecoverService recoverService,
        Lazy<IAccountRepository> rep,
        ILogger<EmailController> logger) : ControllerBase
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

        [HttpGet("send-recover")]
        public async Task<IActionResult> SendRecoverEmail([FromQuery] string loginOrEmail)
        {
            // return Ok in any case
            var user = await rep.Value
                .FindByLoginOrEmail(loginOrEmail)
                .FirstOrDefaultAsync();
            if(user?.IsEmailConfirmed != true)
                return Ok();

            try
            {
                await recoverService.SendRecoverEmail(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return Ok();
        }

        [HttpPost("recover")]
        public async Task<IActionResult> RecoverEmail(PasswordRecover dto)
        {
            await recoverService.Recover(dto.Base64Token, dto.Password);
            return Ok();
        }
    }
}