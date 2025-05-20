using System.ComponentModel.DataAnnotations;

namespace TripNest.Models  // Replace with your actual project namespace
{
    public class UserLoginViewModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }

}