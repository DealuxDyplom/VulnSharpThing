namespace VulnerableApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Уязвимость: Пароль в plain text
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public string? CreditCard { get; set; } // Уязвимость: Чувствительные данные без шифрования
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

    public class SearchModel
    {
        public string Query { get; set; } = string.Empty;
    }

    public class FileModel
    {
        public string FileName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}