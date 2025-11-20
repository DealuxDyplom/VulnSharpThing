using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace VulnSharpThing.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnvController : ControllerBase
    {
        // /Env/run?name=PATH&value=/tmp/fakebin:%25PATH%25&cmd=whoami
        [HttpGet("run")]
        public IActionResult Run([FromQuery] string name, [FromQuery] string value, [FromQuery] string cmd = "whoami")
        {
            var psi = new ProcessStartInfo
            {
                FileName = cmd,              // исполняемый файл фиксирован пользователем запроса (ещё один риск)
                RedirectStandardOutput = true,
                UseShellExecute = false      // важно для EnvironmentVariables
            };

            // ❌ УЯЗВИМО: переменная окружения формируется из недоверенного ввода
            psi.EnvironmentVariables[name] = value;

            using var p = Process.Start(psi)!;
            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return Ok(new { env = $"{name}={value}", cmd, output });
        }
    }
}
