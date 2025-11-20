using Microsoft.EntityFrameworkCore;
using VulnerableApp.Data;
using VulnerableApp.Models;

namespace VulnerableApp.Services
{
    public interface IUserService
    {
        Task<User?> Authenticate(string username, string password);
        Task<List<User>> SearchUsers(string query);
        Task<User?> GetUserById(int id);
        Task<bool> RegisterUser(User user);
        Task<List<User>> GetAllUsers();
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Уязвимость: SQL Injection через конкатенацию строк (эмуляция)
        public async Task<List<User>> SearchUsers(string query)
        {
            // Эмуляция SQL Injection - уязвимый код
            try
            {
                // Для InMemory database эмулируем уязвимое поведение
                var allUsers = await _context.Users.ToListAsync();

                // Уязвимость: Фильтрация на стороне клиента, но демонстрирует концепцию
                var filteredUsers = allUsers.Where(u =>
                    u.Username.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

                // Логируем "уязвимый" запрос
                _logger.LogWarning($"⚠️ VULNERABLE SQL: SELECT * FROM Users WHERE Username LIKE '%{query}%' OR Email LIKE '%{query}%'");

                return filteredUsers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SearchUsers");
                return new List<User>();
            }
        }

        // Альтернативная реализация с реальной уязвимостью для SQL Server
        public async Task<List<User>> SearchUsersVulnerable(string query)
        {
            // Этот метод будет работать только с реальной SQL базой данных
            // Раскомментируйте если используете SQL Server

            // var sql = $"SELECT * FROM Users WHERE Username LIKE '%{query}%' OR Email LIKE '%{query}%'";
            // return await _context.Users.FromSqlRaw(sql).ToListAsync();

            return await SearchUsers(query); // fallback
        }

        // Уязвимость: Небезопасная аутентификация
        public async Task<User?> Authenticate(string username, string password)
        {
            // Уязвимость: Пароль проверяется в plain text
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Уязвимость: Логирование чувствительных данных
                _logger.LogInformation($"User {username} logged in with password: {password}");
            }

            return user;
        }

        // Уязвимость: IDOR (Insecure Direct Object Reference)
        public async Task<User?> GetUserById(int id)
        {
            // Уязвимость: Нет проверки прав доступа
            return await _context.Users.FindAsync(id);
        }

        public async Task<List<User>> GetAllUsers()
        {
            // Уязвимость: Возвращаем всех пользователей без фильтрации
            return await _context.Users.ToListAsync();
        }

        // Уязвимость: Небезопасная десериализация и нет валидации
        public async Task<bool> RegisterUser(User user)
        {
            try
            {
                // Уязвимость: Нет валидации входных данных
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return false;
            }
        }
    }
}