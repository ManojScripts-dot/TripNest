using System;
using System.ComponentModel.DataAnnotations;

namespace TripNest.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }
        public virtual Booking? Booking { get; set; }

        [Required]
        public int UserId { get; set; }
        public virtual User? User { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comments { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
