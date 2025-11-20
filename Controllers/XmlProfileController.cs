using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VulnSharpThing.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class XmlProfileController : ControllerBase
    {
        // Пример: /XmlProfile/save?username=alice
        // ВНИМАНИЕ: уязвимо для XML Injection (S6399)
        [HttpGet("save")]
        public async Task<IActionResult> Save([FromQuery] string username)
        {
            var sb = new StringBuilder();
            // ❌ Небезопасно: сырая подстановка user-controlled значения в XML
            sb.Append($@"
<user>
  <username>{username}</username>
  <role>user</role>
</user>");

            var path = Path.Combine(Path.GetTempPath(), "profile.xml");
            await System.IO.File.WriteAllTextAsync(path, sb.ToString(), Encoding.UTF8);

            return Ok(new { saved = true, path });
        }

        // для наглядной проверки
        [HttpGet("read")]
        public IActionResult Read()
        {
            var path = Path.Combine(Path.GetTempPath(), "profile.xml");
            var xml = System.IO.File.Exists(path) ? System.IO.File.ReadAllText(path, Encoding.UTF8) : "(no file)";
            return Content(xml, "application/xml; charset=utf-8");
        }
    }
}
