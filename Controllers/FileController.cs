using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace VulnSharpThing.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private const string BaseDir = "C:\\VulnSharpFiles\\";

        [HttpGet("read")]
        public IActionResult ReadFile(string filename)
        {
            // Уязвимая конкатенация пользовательского ввода
            string path = Path.Combine(BaseDir, filename);
            // Нет проверки, что файл находится в BaseDir
            string content = System.IO.File.ReadAllText(path);
            return Ok(new { filename = filename, content = content });
        }
    }
}
