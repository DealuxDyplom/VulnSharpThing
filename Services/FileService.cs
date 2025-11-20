namespace VulnerableApp.Services
{
    public interface IFileService
    {
        Task<string> ReadFile(string filePath);
        Task<bool> UploadFile(IFormFile file, string uploadPath);
        Task<bool> DeleteFile(string filePath);
    }

    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;

        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
        }

        // Уязвимость: Path Traversal
        public async Task<string> ReadFile(string filePath)
        {
            // КРИТИЧЕСКАЯ УЯЗВИМОСТЬ: Нет проверки пути к файлу
            _logger.LogInformation($"Reading file: {filePath}");
            return await File.ReadAllTextAsync(filePath);
        }

        // Уязвимость: Загрузка файлов без проверки
        public async Task<bool> UploadFile(IFormFile file, string uploadPath)
        {
            // Уязвимость: Нет проверки типа файла, размера, имени
            var fullPath = Path.Combine(uploadPath, file.FileName);

            // Уязвимость: Перезапись системных файлов возможна
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation($"File uploaded: {fullPath}");
            return true;
        }

        // Уязвимость: Удаление файлов без проверки
        public async Task<bool> DeleteFile(string filePath)
        {
            // Уязвимость: Нет проверки прав доступа к файлу
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation($"File deleted: {filePath}");
                return true;
            }
            return false;
        }
    }
}