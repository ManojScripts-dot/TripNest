using System.ComponentModel.DataAnnotations;

namespace TripNest.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public required  string Name { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public required string Email { get; set; }

        [Required, StringLength(1000)]
        public required  string Message { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}
