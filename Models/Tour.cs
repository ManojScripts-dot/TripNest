using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TripNest.Models
{
    public class Tour
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = null!;

        [Range(1, 365)]
        public int DurationDays { get; set; }  // Number of days for the tour

        [Range(1, 5)]
        public int StarRating { get; set; }   // 1 to 5 stars

        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }    // Tour price

        [StringLength(255)]
        public string ImageUrl { get; set; } = null!;  // URL or relative path to image

        [StringLength(100)]
        public string Destination { get; set; } = null!;  // e.g., Bali, Indonesia

        [StringLength(20)]
        public string Status { get; set; } = "Active";  // e.g., Active, Draft, Inactive

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
