using AspNetCore.Localizer.Json.Localizer;
using ForumApi.Locales;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ForumApi.Controllers
{
    [ApiController]
    [Route("tmp")]
    public class TestController(IJsonStringLocalizer<Message> l) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> TestLocaliztion() 
        {
            return Ok(l["holla"]);
        }
    }
}