using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TripNest.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
        
        public string? Address { get; set; }
        
        public string? Country { get; set; }
        
        public string? PreferredLanguage { get; set; }
        
        // Changed from ProfileImageUrl to ProfileImage file upload
        [Display(Name = "Profile Image")]
        public IFormFile? ProfileImage { get; set; }
        
        // Keep this for internal use (storing the uploaded image URL)
        public string? ProfileImageUrl { get; set; }
    }
}