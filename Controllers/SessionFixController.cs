using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace VulnSharpThing.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SessionFixController : ControllerBase
    {
        // GET /SessionFix/issue?cookie=ATTACKER_SESSION_ID
        [HttpGet("issue")]
        public IActionResult Issue([FromQuery] string cookie)
        {
            // ❌ УЯЗВИМО: назначаем сессионную cookie из недоверенного ввода
            Response.Cookies.Append(".Vuln.Session", cookie); // sink (S6287)
            return Ok(new { set = true, value = cookie });
        }

        // Для наглядности — эхо текущего значения
        [HttpGet("me")]
        public IActionResult Me()
        {
            var v = Request.Cookies[".Vuln.Session"];
            return Ok(new { session = v ?? "(none)" });
        }
    }
}
