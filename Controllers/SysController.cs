using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace VulnSharpThing.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SysController : ControllerBase
    {
        // GET /Sys/search?pattern=*.log
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string pattern)
        {
            // ❌ УЯЗВИМО: небезопасная конкатенация аргументов
            var psi = new ProcessStartInfo
            {
                FileName = "/usr/bin/find",           // бинарь фиксирован (не S2076)
                Arguments = "/var/www -name " + pattern,  // но pattern не экранирован (S5883)
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            using var p = Process.Start(psi)!;
            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return Ok(new { cmd = psi.FileName, args = psi.Arguments, output });
        }
    }
}
