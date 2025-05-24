// Models/Review.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace TripNest.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; } = null!;

        [Required]
        public int UserId { get; set; }
        public virtual User? User { get; set; }

        [Required]
        [Range(1, 5)]
        public int Stars { get; set; }

        [StringLength(1000)]
        public string? Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
