using System.ComponentModel.DataAnnotations;

namespace TripNest.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public string Role { get; set; } = "User"; // Default role is "User"
    }
}
