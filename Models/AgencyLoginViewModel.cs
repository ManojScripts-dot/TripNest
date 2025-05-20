using System.ComponentModel.DataAnnotations;

namespace TripNest.Models  // Replace with your actual namespace
{
    public class AgencyLoginViewModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
