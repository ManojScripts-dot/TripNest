namespace TripNest.Services
{
    public static class ViewHelperExtensions
    {
        public static string GetImageUrl(this string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return "https://res.cloudinary.com/dtudiub1v/image/upload/v1752921619/default-tour_clzafv.jpg";
            }

            // If it's already a full URL (Cloudinary or other), return as is
            if (imagePath.StartsWith("http"))
            {
                return imagePath;
            }

            // If it's a local path, convert to Cloudinary default
            if (imagePath.StartsWith("/images/") || imagePath.Contains("default-tour"))
            {
                return "https://res.cloudinary.com/dtudiub1v/image/upload/v1752921619/default-tour_clzafv.jpg";
            }

            return imagePath;
        }
    }
}