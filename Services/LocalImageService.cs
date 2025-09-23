using Microsoft.AspNetCore.Hosting;

namespace TripNest.Services
{
    public class LocalImageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<LocalImageService> _logger;

        public LocalImageService(IWebHostEnvironment environment, ILogger<LocalImageService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string> SaveImageAsync(IFormFile file, string folder = "uploads")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("No file provided");
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new ArgumentException($"File type {fileExtension} is not supported. Allowed types: {string.Join(", ", allowedExtensions)}");
                }

                // Validate file size (10MB max)
                if (file.Length > 10 * 1024 * 1024)
                {
                    throw new ArgumentException("File size exceeds 10MB limit");
                }

                // Create uploads directory if it doesn't exist
                var uploadsPath = Path.Combine(_environment.WebRootPath, folder);
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Generate unique filename
                var fileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString("N")[..8]}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative URL
                var relativeUrl = $"/{folder}/{fileName}";
                _logger.LogInformation("Image saved locally: {FilePath}, URL: {RelativeUrl}", filePath, relativeUrl);
                
                return relativeUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving image locally");
                throw;
            }
        }

        public void DeleteImage(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl) || !imageUrl.StartsWith("/"))
                {
                    return; // Not a local image
                }

                var filePath = Path.Combine(_environment.WebRootPath, imageUrl.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("Local image deleted: {FilePath}", filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting local image: {ImageUrl}", imageUrl);
            }
        }
    }
}
