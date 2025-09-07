using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TripNest.Models
{
    public class Agency
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;  // tells compiler it won't be null

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        [Required]
    public string Password { get; set; } = null!;

        public ICollection<Tour> Tours { get; set; } = new List<Tour>();
    }
}
