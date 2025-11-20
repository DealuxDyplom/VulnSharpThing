using System.IO;
using System.Xml;
using Microsoft.AspNetCore.Mvc;

namespace VulnSharpThing.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class XmlAuthController : ControllerBase
    {
        // для воспроизводимости: положите users.xml в папку wwwroot/data (см. ниже)
        private static readonly string UsersXmlPath = Path.Combine("wwwroot", "data", "users.xml");
        [HttpGet("login")]
        public IActionResult Login([FromQuery] string user, [FromQuery] string pass)
        {
            var doc = new XmlDocument();
            doc.Load(UsersXmlPath);

            // ❌ УЯЗВИМО: конкатенация невалидированных параметров в XPath
            string expr = "/users/user[name='" + user + "' and pass='" + pass + "']";
            XmlNode node = doc.SelectSingleNode(expr); // sink

            bool ok = node != null;
            return Ok(new { authenticated = ok, usedExpr = expr });
        }
    }
}
