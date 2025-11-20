using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace VulnSharpThing.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SysArgsController : ControllerBase
    {
        // GET /SysArgs/search?pattern=*.log
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string pattern)
        {
            // ❌ Security Hotspot (S6350): недоверенный ввод уходит в аргументы команды
            var psi = new ProcessStartInfo
            {
                FileName = "/usr/bin/find",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            // Показываем именно риск аргументов:
            // 1) Через Arguments-строку
            psi.Arguments = "/var/www -name " + pattern;

            // 2) (или) через ArgumentList — тоже риск при недоверенных данных
            // psi.ArgumentList.Add("/var/www");
            // psi.ArgumentList.Add("-name");
            // psi.ArgumentList.Add(pattern); // см. предупреждение Microsoft: use only with trusted data

            using var p = Process.Start(psi)!;
            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return Ok(new { cmd = psi.FileName, args = psi.Arguments, output });
        }
    }
}
