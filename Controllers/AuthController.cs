using Microsoft.AspNetCore.Mvc;
using VulnerableApp.Models;
using VulnerableApp.Services;

namespace VulnerableApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Login()
        {
            return View();
        }

        // Уязвимость: Небезопасная аутентификация
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.Authenticate(model.Username, model.Password);
                if (user != null)
                {
                    // Уязвимость: Слабый session management
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("Role", user.Role);

                    // Уязвимость: Cookies с длительным сроком действия
                    var cookieOptions = new CookieOptions
                    {
                        Expires = model.RememberMe ? DateTime.Now.AddYears(1) : DateTime.Now.AddHours(1),
                        HttpOnly = false, // Уязвимость
                        Secure = false,   // Уязвимость
                        SameSite = SameSiteMode.None
                    };

                    Response.Cookies.Append("AuthToken", user.Id.ToString(), cookieOptions);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid username or password");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            // Уязвимость: Неполное уничтожение сессии
            HttpContext.Session.Clear();
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        // Уязвимость: Уязвимости сброса пароля
        [HttpPost]
        public IActionResult ResetPassword(string email)
        {
            // Уязвимость: Раскрытие информации о существовании email
            var exists = true; // В реальном приложении здесь была бы проверка

            if (exists)
            {
                // Уязвимость: Слабый токен сброса
                var resetToken = Guid.NewGuid().ToString().Substring(0, 6); // Слишком короткий токен
                ViewBag.Message = $"Reset token sent: {resetToken}";
            }
            else
            {
                ViewBag.Message = "If the email exists, a reset token will be sent.";
            }

            return View();
        }
    }
}