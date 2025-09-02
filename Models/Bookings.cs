using System;
using System.ComponentModel.DataAnnotations;

namespace TripNest.Models
{
    public class Booking
    {
        public Booking()
        {
            CustomerName = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            SpecialRequests = string.Empty;
            AdditionalNotes = string.Empty;
            Status = "Pending";
        }

        public int Id { get; set; }

        [Required]
        public int TourId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Number of Guests")]
        public int NumberOfGuests { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Preferred Tour Date")]
        public DateTime PreferredTourDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Special Requests")]
        public string SpecialRequests { get; set; }

        [StringLength(500)]
        [Display(Name = "Additional Notes")]
        public string AdditionalNotes { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        // Navigation properties

        public virtual Tour? Tour { get; set; }

        // New foreign key to User
        [Required]
        public int UserId { get; set; }

        public virtual User? User { get; set; }
    }
}
