using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace VulnSharpThing.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileOracleController : ControllerBase
    {
        private const string BaseDir = "C:\\VulnData"; // намеренно без завершающего слеша

        // /FileOracle/exists?file=report.txt
        [HttpGet("exists")]
        public IActionResult Exists([FromQuery] string file)
        {
            // ❌ УЯЗВИМО: нет каноникализации/валидации пути, возвращаем разный статус
            var path = Path.Combine(BaseDir, file);
            if (!System.IO.File.Exists(path))           // sink (leaks existence)
                return NotFound(new { path, exists = false });

            return Ok(new { path, exists = true });
        }
    }
}
