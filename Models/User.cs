using System.ComponentModel.DataAnnotations;

namespace TripNest.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        public string Role { get; set; } = "User"; // Default role is "User"

        [Required]
        public UserProfile UserProfile { get; set; } = null!;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
