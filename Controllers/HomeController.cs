using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VulnerableApp.Models;
using VulnerableApp.Services;

namespace VulnerableApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IUserService userService, IFileService fileService, ILogger<HomeController> logger)
        {
            _userService = userService;
            _fileService = fileService;
            _logger = logger;
        }

        // Уязвимость: XSS
        public IActionResult Index()
        {
            // Уязвимость: Данные из query string без кодирования
            ViewBag.SearchQuery = HttpContext.Request.Query["search"];
            return View();
        }

        // Уязвимость: SQL Injection демо
        [HttpPost]
        public async Task<IActionResult> Search(SearchModel model)
        {
            if (!string.IsNullOrEmpty(model.Query))
            {
                var results = await _userService.SearchUsers(model.Query);
                return View("SearchResults", results);
            }
            return RedirectToAction("Index");
        }

        // Уязвимость: Path Traversal
        [HttpGet]
        public async Task<IActionResult> ViewFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest();

            // Уязвимость: Прямая передача пользовательского ввода в файловую систему
            var basePath = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(basePath, "uploads", fileName);

            try
            {
                var content = await _fileService.ReadFile(filePath);
                ViewBag.FileContent = content;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // Уязвимость: Небезопасная загрузка файлов
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                // Создаем директорию, если не существует
                Directory.CreateDirectory(uploadPath);
                await _fileService.UploadFile(file, uploadPath);
                ViewBag.Message = "File uploaded successfully!";
            }
            return View("Index");
        }

        // Уязвимость: XXE и небезопасная десериализация
        [HttpPost]
        public IActionResult ProcessData(string jsonData)
        {
            try
            {
                // Уязвимость: Небезопасная десериализация
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                var data = JsonSerializer.Deserialize<object>(jsonData, options);
                return Json(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // Уязвимость: IDOR
        [HttpGet]
        public async Task<IActionResult> UserProfile(int userId)
        {
            // Уязвимость: Нет проверки прав доступа к данным другого пользователя
            var user = await _userService.GetUserById(userId);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // Уязвимость: CSRF (отключена защита)
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User user)
        {
            // Уязвимость: Нет проверки подлинности запроса
            await _userService.RegisterUser(user);
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Уязвимость: Раскрытие детальной информации об ошибке
            return View();
        }
    }
}