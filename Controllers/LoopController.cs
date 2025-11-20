using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;

namespace VulnSharpThing.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoopController : ControllerBase
    {
        // Пример: /Loop/compute?count=10000000
        [HttpGet("compute")]
        public IActionResult Compute([FromQuery] int count)
        {
            // ❌ S6680: недоверенный ввод определяет границы итераций

            // 1) "Функция-цикл": Enumerable.Range
            var sum = Enumerable.Range(1, count)  // потенциально огромный диапазон
                                .Sum();

            // 2) Классический цикл
            var sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                // имитация работы (логирование/сбор строки/вызов репозитория и т. п.)
                if (i % 1000 == 0) sb.Append(i).Append(',');
            }

            return Ok(new { count, sum, sample = sb.ToString() });
        }
    }
}
