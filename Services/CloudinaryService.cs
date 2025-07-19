using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace TripNest.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder = "tours");
        Task<bool> DeleteImageAsync(string publicId);
        string ExtractPublicIdFromUrl(string cloudinaryUrl);
    }

    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(IConfiguration configuration, ILogger<CloudinaryService> logger)
        {
            _logger = logger;
            
            // Try to get from environment variables first, then fall back to configuration
            var cloudName = Environment.GetEnvironmentVariable("Cloudinary__CloudName") 
                           ?? configuration["Cloudinary:CloudName"];
            var apiKey = Environment.GetEnvironmentVariable("Cloudinary__ApiKey") 
                        ?? configuration["Cloudinary:ApiKey"];
            var apiSecret = Environment.GetEnvironmentVariable("Cloudinary__ApiSecret") 
                           ?? configuration["Cloudinary:ApiSecret"];

            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                throw new ArgumentException("Cloudinary configuration is missing. Please set environment variables: Cloudinary__CloudName, Cloudinary__ApiKey, Cloudinary__ApiSecret");
            }

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
            
            _logger.LogInformation("Cloudinary service initialized with CloudName: {CloudName}", cloudName);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder = "tours")
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is required");
            }

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                throw new ArgumentException("Invalid file type. Only JPEG, PNG, GIF, and WebP are allowed.");
            }

            // Validate file size (10MB max)
            if (file.Length > 10 * 1024 * 1024)
            {
                throw new ArgumentException("File size must be less than 10MB");
            }

            try
            {
                using var stream = file.OpenReadStream();
                
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder,
                    Transformation = new Transformation()
                        .Quality("auto")
                        .FetchFormat("auto")
                        .Width(1200)
                        .Height(800)
                        .Crop("fill")
                        .Gravity("auto"),
                    PublicId = $"{folder}_{DateTime.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString("N")[..8]}"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    _logger.LogError("Cloudinary upload error: {Error}", uploadResult.Error.Message);
                    throw new Exception($"Image upload failed: {uploadResult.Error.Message}");
                }

                _logger.LogInformation("Image uploaded successfully: {Url}, PublicId: {PublicId}", 
                    uploadResult.SecureUrl.ToString(), uploadResult.PublicId);
                
                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to Cloudinary");
                throw;
            }
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                _logger.LogWarning("PublicId is empty, skipping image deletion");
                return false;
            }

            try
            {
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);
                
                _logger.LogInformation("Image deletion result for {PublicId}: {Result}", publicId, result.Result);
                return result.Result == "ok";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image from Cloudinary with publicId: {PublicId}", publicId);
                return false;
            }
        }

        public string ExtractPublicIdFromUrl(string cloudinaryUrl)
        {
            if (string.IsNullOrEmpty(cloudinaryUrl))
                return string.Empty;

            try
            {
                // Check if it's a Cloudinary URL
                if (!cloudinaryUrl.Contains("cloudinary.com"))
                    return string.Empty;

                var uri = new Uri(cloudinaryUrl);
                var path = uri.AbsolutePath;
                
                // Split the path to get segments
                var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                
                // Find the image segment (usually after /image/upload/ or /video/upload/)
                int startIndex = -1;
                for (int i = 0; i < segments.Length; i++)
                {
                    if (segments[i] == "upload")
                    {
                        startIndex = i + 1;
                        break;
                    }
                }

                if (startIndex == -1 || startIndex >= segments.Length)
                    return string.Empty;

                // Skip version if present (starts with 'v' followed by numbers)
                if (startIndex < segments.Length && segments[startIndex].StartsWith("v") && 
                    segments[startIndex].Length > 1 && char.IsDigit(segments[startIndex][1]))
                {
                    startIndex++;
                }

                if (startIndex >= segments.Length)
                    return string.Empty;

                // Get the remaining path segments
                var pathSegments = segments.Skip(startIndex).ToList();
                
                // Remove file extension from the last segment
                if (pathSegments.Count > 0)
                {
                    var lastSegment = pathSegments.Last();
                    var extensionIndex = lastSegment.LastIndexOf('.');
                    if (extensionIndex > 0)
                    {
                        pathSegments[pathSegments.Count - 1] = lastSegment.Substring(0, extensionIndex);
                    }
                }

                var publicId = string.Join("/", pathSegments);
                _logger.LogDebug("Extracted PublicId: {PublicId} from URL: {Url}", publicId, cloudinaryUrl);
                
                return publicId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting public ID from Cloudinary URL: {Url}", cloudinaryUrl);
                return string.Empty;
            }
        }
    }
}