using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace VulnSharpThing.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReflectController : ControllerBase
    {
        // Пример: /Reflect/run?type=System.IO.FileInfo&method=ToString&arg=C:\Windows\win.ini
        [HttpGet("run")]
        public IActionResult Run([FromQuery] string type, [FromQuery] string method, [FromQuery] string? arg = null)
        {
            // ❌ УЯЗВИМО: имена типа/метода полностью контролируются пользователем
            var t = Type.GetType(type, throwOnError: false);         // sink #1
            if (t == null) return BadRequest("type not found");

            object? instance = null;
            if (!t.IsAbstract && !t.IsSealed)                        // для инстанцируемых типов
                instance = Activator.CreateInstance(t);              // sink #2

            var mi = t.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (mi == null) return BadRequest("method not found");

            var parameters = mi.GetParameters().Length == 0 ? Array.Empty<object?>() : new object?[] { arg };
            var result = mi.Invoke(instance, parameters);            // sink #3

            return Ok(new { type, method, arg, result = result?.ToString() });
        }
    }
}
