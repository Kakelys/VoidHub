using ForumApi.DTO.DForum;
using Microsoft.AspNetCore.Mvc;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("api/v1/tests")]
    public class TestController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> ForumEditVdalitionTest(ForumEdit dto)
        {
            return Ok();
        }
    }
}