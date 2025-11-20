using Microsoft.AspNetCore.Mvc;

namespace VulnSharpThing.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StacktraceController : ControllerBase
    {
        // /Stacktrace/boom?msg=SomethingBad
        [HttpGet("boom")]
        public IActionResult ExceptionEndpoint([FromQuery] string msg = "Boom")
        {
            try
            {
                throw new InvalidOperationException(msg);
            }
            catch (Exception ex)
            {
                // ❌ УЯЗВИМО: раскрываем стек исключения во внешнем ответе
                return Content(ex.StackTrace ?? "no stack", "text/plain");
            }
        }
    }
}
